<<<<<<<< HEAD:src/Bcl/src/CoreLib/CodeGeneration/TypePath.cs
﻿using Library.CodeGeneration.Models;
using Library.DesignPatterns.Markers;

using GenericTypeInfo = (Library.CodeGeneration.TypePath Type, string? Constraints);

namespace Library.CodeGeneration;

[Immutable]
public sealed class TypePath : IEquatable<TypePath>
{
    public TypePath(in string? name, in string? nameSpace = null) =>
        (this.Name, this.NameSpace) = SplitTypePath(Validate(name), nameSpace);

    public string FullPath
    {
        get
        {
            if (this.Name.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var result = this.NameSpace.IsNullOrEmpty() ? this.Name : $"{this.NameSpace}.{this.Name}";
            if (this.GenericTypes.Any())
            {
                result = $"{result}<{StringHelper.Merge(this.GenericTypes.Select(x => x.Type.FullPath), ',')}>";
            }
            return result;
        }
    }

    public ISet<GenericTypeInfo> GenericTypes { get; } = new HashSet<GenericTypeInfo>();
    public string? Name { get; }
========
﻿//using Library.DesignPatterns.Markers;

//namespace Library.CodeGeneration.v2.Back;

//[Immutable]
//public readonly struct TypePath : IEquatable<TypePath>
//{
//    public TypePath(in string? name, in string? nameSpace = null) =>
//        (this.Name, this.NameSpace) = SplitTypePath(Validate(name), nameSpace);

//    public string FullPath
//    {
//        get
//        {
//            if (this.Name.IsNullOrEmpty())
//            {
//                return string.Empty;
//            }
//            var result = this.NameSpace.IsNullOrEmpty() ? this.Name : $"{this.NameSpace}.{this.Name}";
//            if (this.GenericTypes.Any())
//            {
//                result = $"{result}<{StringHelper.Merge(this.GenericTypes.Select(x => x.FullPath), ',')}>";
//            }
//            return result;
//        }
//    }

//    public ISet<TypePath> GenericTypes { get; } = new HashSet<TypePath>();

//    public string? Name { get; }
>>>>>>>> af239dbbe0a4bca0be3c60fc816e389a157964b1:src/Bcl/src/CoreLib/CodeGeneration/v2/Back/TypePath.cs

//    public string? NameSpace { get; }

<<<<<<<< HEAD:src/Bcl/src/CoreLib/CodeGeneration/TypePath.cs
    public static string Combine(string part1, params string[] parts) =>
        new StringBuilder(part1).AppendAll(parts, part => $".{part.Trim('.')}").ToString();

    public static implicit operator string?(in TypePath typeInfo) =>
        typeInfo.ToString();
========
//    public static implicit operator string?(in TypePath typeInfo) =>
//        typeInfo.ToString();
>>>>>>>> af239dbbe0a4bca0be3c60fc816e389a157964b1:src/Bcl/src/CoreLib/CodeGeneration/v2/Back/TypePath.cs

//    public static implicit operator TypePath(in string? typeInfo) =>
//        new(typeInfo);

<<<<<<<< HEAD:src/Bcl/src/CoreLib/CodeGeneration/TypePath.cs
    public static TypePath New(in string? name, in string? nameSpace = null) =>
        new(name, nameSpace);

    public static TypePath New(in Type? type) =>
        new(type?.FullName);
========
//    public static string Merge(string part1, params string[] parts)
//    {
//        var result = new StringBuilder(part1);
//        foreach (var part in parts)
//        {
//            _ = result.Append(part.EndsWith('.') ? part : string.Concat(part, "."));
//        }
//        return result.ToString();
//    }

//    public static TypePath New(in string? name, in string? nameSpace = null) =>
//        new(name, nameSpace);

//    public static TypePath New((string? Name, string? NameSpace) typePath) =>
//        new(typePath.Name, typePath.NameSpace);

//    public static TypePath New(in Type? type) =>
//        new(type?.FullName);
>>>>>>>> af239dbbe0a4bca0be3c60fc816e389a157964b1:src/Bcl/src/CoreLib/CodeGeneration/v2/Back/TypePath.cs

//    public static TypePath New<TType>() =>
//        new(typeof(TType).FullName);

//    public static bool operator !=(TypePath left, TypePath right) =>
//        !(left == right);

//    public static bool operator ==(TypePath left, TypePath right) =>
//        left.Equals(right);

//    public static (string? Name, string? NameSpace) SplitTypePath(in string? typePath)
//    {
//        if (typePath.IsNullOrEmpty())
//        {
//            return default;
//        }

<<<<<<<< HEAD:src/Bcl/src/CoreLib/CodeGeneration/TypePath.cs
        var dotLastIndex = typePath.LastIndexOf('.');
        return dotLastIndex == -1
            ? (typePath, null)
            : (typePath[(dotLastIndex + 1)..], typePath[..dotLastIndex]);
    }
========
//        var dotLastIndex = typePath.LastIndexOf('.');
//        return dotLastIndex == -1 ? ((string? Name, string? NameSpace))(typePath, null) : ((string? Name, string? NameSpace))(typePath[(dotLastIndex + 1)..], typePath[..dotLastIndex]);
//    }
>>>>>>>> af239dbbe0a4bca0be3c60fc816e389a157964b1:src/Bcl/src/CoreLib/CodeGeneration/v2/Back/TypePath.cs

//    public static (string? Name, string? NameSpace) SplitTypePath(in string? name, in string? nameSpace = null) =>
//        string.IsNullOrEmpty(nameSpace)
//            ? SplitTypePath(name)
//            : nameSpace.EndsWith('.') ? SplitTypePath($"{nameSpace}{name}") : SplitTypePath($"{nameSpace}.{name}");

<<<<<<<< HEAD:src/Bcl/src/CoreLib/CodeGeneration/TypePath.cs
    public TypePath AddGenericType(GenericTypeInfo genericType)
    {
        _ = this.GenericTypes.Add(genericType);
        return this;
    }
    public TypePath AddGenericType(string genericType)
    {
        _ = this.GenericTypes.Add((genericType, null));
        return this;
    }
========
//    public TypePath AddGenericType(TypePath typePath)
//    {
//        _ = this.GenericTypes.Add(typePath);
//        return this;
//    }
>>>>>>>> af239dbbe0a4bca0be3c60fc816e389a157964b1:src/Bcl/src/CoreLib/CodeGeneration/v2/Back/TypePath.cs

//    public void Deconstruct(out string? name, out string? nameSpace) =>
//        (name, nameSpace) = (this.Name, this.NameSpace);

//    public bool Equals(TypePath? other) =>
//        (this.Name, this.NameSpace) == (other?.Name, other?.NameSpace);

//    public override bool Equals(object? obj) =>
//        obj is TypePath path && this.Equals(path);

<<<<<<<< HEAD:src/Bcl/src/CoreLib/CodeGeneration/TypePath.cs
    public override int GetHashCode() =>
        HashCode.Combine(this.Name?.GetHashCode() ?? 0, this.NameSpace?.GetHashCode() ?? 0);
========
//    public bool Equals(TypePath other) =>
//        (this.Name, this.NameSpace) == (other.Name, other.NameSpace);

//    public override int GetHashCode() =>
//        HashCode.Combine(this.Name?.GetHashCode() ?? 0, this.NameSpace?.GetHashCode() ?? 0);
>>>>>>>> af239dbbe0a4bca0be3c60fc816e389a157964b1:src/Bcl/src/CoreLib/CodeGeneration/v2/Back/TypePath.cs

//    public override string ToString() =>
//        this.FullPath;

//    private static string? Validate(string? name) =>
//        name?.Contains('`') is null or false ? name : name[..name.IndexOf('`')];
//}