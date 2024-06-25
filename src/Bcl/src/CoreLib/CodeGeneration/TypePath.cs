using System.Collections;
using System.Diagnostics;
using System.Globalization;

using Library.CodeGeneration.Models;
using Library.DesignPatterns.Markers;
using Library.Validations;

using TypeData = (string Name, string NameSpace, System.Collections.Generic.IEnumerable<Library.CodeGeneration.TypePath> Generics, bool IsNullable);

namespace Library.CodeGeneration;

[DebuggerStepThrough, StackTraceHidden]
[Immutable]
public sealed class TypePath([DisallowNull] in string fullPath, in IEnumerable<string>? generics = null, bool? isNullable = null) : IEquatable<TypePath>
{
    private static readonly Dictionary<Type, string> _primitiveTypes = new()
    {
        { typeof(int), "int" },
        { typeof(long), "long" },
        { typeof(byte), "byte" },
        { typeof(char), "char" },
        { typeof(bool), "bool" },
        { typeof(uint), "uint" },
        { typeof(nint), "nint" },
        { typeof(float), "float" },
        { typeof(nuint), "nuint" },
        { typeof(short), "short" },
        { typeof(sbyte), "sbyte" },
        { typeof(double), "double" },
        { typeof(string), "string" },
        { typeof(ushort), "ushort" },
        { typeof(decimal), "decimal" },
    };

    private readonly TypeData _data = Parse(fullPath, generics, isNullable);
    private string? _fullName;
    private string? _fullPath;

    [NotNull]
    public string FullName => this._fullName ??= this.GetFullName();

    [NotNull]
    public string FullPath => this._fullPath ??= this.GetFullPath();

    [NotNull]
    public IEnumerable<TypePath> Generics => this._data.Generics;

    public bool IsGeneric => this._data.Generics.Any();

    public bool IsNullable => this._data.IsNullable;

    [NotNull]
    public string Name => this._data.Name;

    public string NameSpace => this._data.NameSpace;

    public static string Combine(in string? part1, params string?[] parts)
        => StringHelper.Merge(EnumerableHelper.AsEnumerable(part1).AddRangeImmuted(parts).Compact().Select(x => x.Trim('.')), '.');

    [return: NotNullIfNotNull(nameof(typePath))]
    public static string? GetName(in string? typePath)
        => typePath == null ? null : Parse(typePath).Name;

    [return: NotNullIfNotNull(nameof(typePath))]
    public static string? GetNameSpace(in string? typePath)
        => typePath == null ? null : Parse(typePath).NameSpace;

    public static string? GetNameSpace<T>()
    {
        var path = typeof(T).FullName;
        return path.IsNullOrEmpty() ? string.Empty : Parse(path).NameSpace;
    }

    [return: NotNull]
    public static IEnumerable<string>? GetNameSpaces(in string? typePath)
        => typePath == null ? [] : New(typePath).GetNameSpaces();

    [return: NotNullIfNotNull(nameof(typeInfo))]
    public static implicit operator string?(in TypePath? typeInfo)
        => typeInfo?.FullPath;

    [return: NotNullIfNotNull(nameof(typeInfo))]
    public static implicit operator TypePath?(in string? typeInfo)
        => typeInfo.IsNullOrEmpty() ? null : new(typeInfo);

    [return: NotNullIfNotNull(nameof(typeInfo))]
    public static implicit operator TypePath?(in Type? typeInfo)
        => typeInfo == null ? null : New(typeInfo);

    [return: NotNull]
    public static TypePath New([DisallowNull] in string name, in string? nameSpace)
        => new(Combine(nameSpace, name));

    [return: NotNull]
    public static TypePath New([DisallowNull] in string name, in string? nameSpace, in IEnumerable<string>? generics)
        => new(Combine(nameSpace, name), generics);

    [return: NotNull]
    public static TypePath New([DisallowNull] in string name, in string? nameSpace, in IEnumerable<string>? generics, bool? isNullable)
        => new(Combine(nameSpace, name), generics, null);

    [return: NotNull]
    public static TypePath New([DisallowNull] in string fullPath)
        => new(fullPath);

    [return: NotNull]
    public static TypePath New([DisallowNull] in string fullPath, in IEnumerable<string>? generics)
        => new(fullPath, generics);

    [return: NotNull]
    public static TypePath New([DisallowNull] in string fullPath, bool? isNullable)
        => new(fullPath, null, isNullable);

    [return: NotNull]
    public static TypePath New([DisallowNull] in string fullPath, in IEnumerable<string>? generics, bool? isNullable)
        => new(fullPath, generics, isNullable);

    [return: NotNull]
    public static TypePath New([DisallowNull] in Type type)
        => new(type?.FullName!);

    [return: NotNull]
    public static TypePath New([DisallowNull] in Type type, in IEnumerable<Type>? generics)
        => new(type?.FullName!, generics?.Select(x => x.Name == "Nullable`1" ? $"{x.GenericTypeArguments[0].FullName}?" : x.FullName!));

    [return: NotNull]
    public static TypePath New([DisallowNull] in Type type, in IEnumerable<Type>? generics, bool? isNullable)
        => new(type?.FullName!, generics?.Select(x => x.FullName!), isNullable);

    [return: NotNull]
    public static TypePath New<T>()
        => new(typeof(T).FullName!);

    [return: NotNull]
    public static TypePath New<T>(in IEnumerable<string>? generics)
        => new(typeof(T).FullName!, generics);

    [return: NotNull]
    public static TypePath New<T>(in IEnumerable<string>? generics, bool isNullable)
        => new(typeof(T).FullName!, generics, isNullable);

    [return: NotNull]
    public static TypePath NewTask()
        => New<Task>();

    [return: NotNull]
    public static TypePath NewTask(in TypePath generic)
        => New(typeof(Task<>).FullName!, [generic.FullName]);

    [return: NotNull]
    public static TypePath NewTask(in TypePath generic, bool isNullable)
        => New(typeof(Task<>).FullName!, [generic.FullName], isNullable);

    [return: NotNull]
    public static TypePath NewEnumerable(in TypePath generic)
        => New(typeof(IEnumerable<>).FullName!, [generic]);

    [return: NotNull]
    public static TypePath NewEnumerable()
        => New<IEnumerable>();

    public static bool operator !=(in TypePath? left, in TypePath? right)
        => !(left == right);

    public static bool operator ==(in TypePath? left, in TypePath? right)
        => left?.Equals(right) ?? (right is null);

    public string AsKeyword()
    {
        var keyword = GetKeyword(this.Name);
        if (this.IsGeneric)
        {
            var genericArguments = string.Join(", ", this.Generics.Select(g => g.AsKeyword()));
            keyword = $"{keyword}<{genericArguments}>";
        }
        if (this.IsNullable)
        {
            keyword += "?";
        }
        return keyword;

        static string GetKeyword(string typeName)
            => _primitiveTypes.FirstOrDefault(x => x.Key.Name == typeName).Value ?? typeName;
    }

    public void Deconstruct(out string? name, out string? nameSpace)
        => (name, nameSpace) = (this.Name, this.NameSpace);

    public void Deconstruct(out string? name, out string? nameSpace, out IEnumerable<TypePath> generics)
        => (name, nameSpace, generics) = (this.Name, this.NameSpace, this.Generics);

    public bool Equals(TypePath? other)
        => (this.Name, this.NameSpace) == (other?.Name, other?.NameSpace);

    public override bool Equals(object? obj)
        => obj is TypePath path && this.Equals(path);

    public override int GetHashCode()
        => this.FullName.GetHashCode(StringComparison.Ordinal);

    [return: NotNull]
    public IEnumerable<string> GetNameSpaces()
    {
        return iterate().Compact().Distinct();

        IEnumerable<string> iterate()
        {
            if (!this.NameSpace.IsNullOrEmpty())
            {
                yield return this.NameSpace;
            }

            foreach (var generic in this.Generics)
            {
                foreach (var genericNamespace in generic.GetNameSpaces())
                {
                    yield return genericNamespace;
                }
            }
        }
    }

    [return: NotNull]
    public override string ToString()
        => this.FullName;

    public TypePath WithNullable(bool isNullable)
    {
        if (this.IsNullable == isNullable)
        {
            return this;
        }

        var fullPath = this.GetFullPath().Trim('?');
        if (isNullable)
        {
            fullPath = fullPath.AddEnd('?');
        }
        return new(fullPath);
    }

    private static TypeData Parse(in string typePath, in IEnumerable<string>? generics = null, bool? isNullable = null)
    {
        // Validation checks
        Check.MustBeArgumentNotNull(typePath);
        Check.MustBe(generics?.All(x => !x.IsNullOrEmpty()) ?? true, () => "Generic types cannot be null or empty.");

        // Initializations
        var typePathBuffer = typePath.ArgumentNotNull()
            .Trim()
            .TrimStart("async ")
            .EndsWith('?')
                ? TransformKeyword(typePath.TrimEnd('?')).AddEnd('?')
                : TransformKeyword(typePath);

        var gens = new List<string>();

        // Manage nullability.
        if (isNullable is { } nullable)
        {
            typePathBuffer = typePathBuffer.TrimEnd('?');
            if (nullable)
            {
                typePathBuffer = typePathBuffer.AddEnd('?');
            }
        }

        // 1Nullability` is of output parameter
        var nullability = typePathBuffer.EndsWith('?');

        // No longer nullable sign is required. So remove it.
        typePathBuffer = typePathBuffer.TrimEnd('?');

        // Find generics
        if (typePathBuffer.Contains('<', StringComparison.Ordinal))
        {
            var indexOfGenSymbol = typePathBuffer.IndexOf('<', StringComparison.Ordinal);
            var gen = typePathBuffer[indexOfGenSymbol..].Trim('<').Trim('>');
            typePathBuffer = typePathBuffer[..indexOfGenSymbol];
            gens.AddRange(gen.Split(',').Select(x => x.Trim()));
        }
        if (typePathBuffer.Contains('`', StringComparison.Ordinal))
        {
            typePathBuffer = typePathBuffer.Remove(typePathBuffer.IndexOf('`', StringComparison.Ordinal), 2);
        }
        if (generics?.Any() ?? false)
        {
            gens.AddRange(generics);
        }

        // Retrieve name and namespace
        var lastIndexOfDot = typePathBuffer.LastIndexOf('.');
        (var name, var nameSpace) = lastIndexOfDot > 0
            ? (typePathBuffer[(lastIndexOfDot + 1)..], typePathBuffer[..lastIndexOfDot])
            : (typePathBuffer, string.Empty);

        var genTypes = gens.Select(x => new TypePath(x));

        // To be more friendly, let's be kind and use C# keywords.
        //x (name, nameSpace) = ToKeyword(name, nameSpace);
        return (name, nameSpace, genTypes, nullability);
    }

    private static string TransformKeyword(string keyword)
    {
        var primitive = _primitiveTypes.FirstOrDefault(x => x.Value == keyword);
        return primitive.Key?.FullName != null
            ? primitive.Key.FullName
            : keyword;
    }

    [return: NotNull]
    private string GetFullName()
    {
        var buffer = new StringBuilder();
        _ = buffer.Append(this.Name);
        if (this.Generics.Any())
        {
            _ = buffer.Append('<')
                .Append(this.Generics.Select(x => x.GetFullName())!.Merge(", "))
                .Append('>');
        }
        if (this.IsNullable)
        {
            _ = buffer.Append('?');
        }
        return buffer.ToString();
    }

    [return: NotNull]
    private string GetFullPath()
    {
        var buffer = new StringBuilder();
        if (!this.NameSpace.IsNullOrEmpty())
        {
            _ = buffer.Append(CultureInfo.CurrentCulture, $"{this.NameSpace}.");
        }
        _ = buffer.Append(this.Name);
        if (this.Generics.Any())
        {
            _ = buffer.Append('<')
                .Append(this.Generics.Select(x => x.GetFullPath())!.Merge(", "))
                .Append('>');
        }
        if (this.IsNullable)
        {
            _ = buffer.Append('?');
        }
        return buffer.ToString();
    }
}