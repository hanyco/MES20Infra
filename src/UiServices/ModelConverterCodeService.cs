using System.Diagnostics.CodeAnalysis;

using Contracts.Services;

using HanyCo.Infra.CodeGeneration.Definitions;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
using Library.Interfaces;
using Library.Results;
using Library.Validations;

namespace Services;

internal sealed class ModelConverterCodeService(ICodeGeneratorEngine codeGenerator) : IBusinessService, IModelConverterCodeService
{
    private readonly ICodeGeneratorEngine _codeGenerator = codeGenerator;

    /// <summary>
    /// This method generates codes based on the provided parameters.
    /// </summary>
    /// <param name="args">The parameters for code generation.</param>
    /// <returns>A result containing the generated codes.</returns>
    public Result<Codes> GenerateCodes(ModelConverterCodeParameter args)
    {
        // Validate the input parameters.
        if (!validate(args).TryParse(out var vr))
        {
            return vr.WithValue(Codes.Empty);
        }

        // Deconstruct the input parameters.
        (var dto, var srcClass, var dstClass, var methodName) = args;

        // If methodName is null, use a default method name.
        methodName ??= CodeConstants.Converter_Convert_MethodName(dstClass.Name!);

        // Create a single DTO converter.
        var ma = createSingleDtoConverter(dto, srcClass, dstClass, methodName);

        // Create an enumerable converter.
        var mb = createEnumerableConverter(srcClass, dstClass, methodName);

        // Create an extension class and add members.
        var cl = createExtensionClassAndAddMembers(srcClass, ma, mb);

        // Create a namespace.
        var ns = createNameSpace(dto);

        // Add the class to the namespace.
        AddClassToNameSpace(cl, ns);

        // Add necessary using statements
        AddUsings(dto, ma, mb, ns);

        // Generate the code.
        var codeGenRes = this._codeGenerator.Generate(ns);

        // If code generation failed, return an empty result.
        if (codeGenRes.IsFailure)
        {
            return codeGenRes.WithValue(Codes.Empty);
        }

        // Create a new code object and set its properties.
        var result = Code.New(methodName, Languages.CSharp, codeGenRes, true, $"ModelConverter.{srcClass}.{methodName}.cs")
            .With(x => x.props().Category = CodeCategory.Converter)
            .ToCodes();

        // Return the result.
        return Result<Codes>.CreateSuccess(result);

        // Validate the input parameters.
        [return: NotNull]
        static Result validate([NotNull] ModelConverterCodeParameter? viewModel) => viewModel.ArgumentNotNull()
                .Check()
                .NotNull(x => x.SourceDto)
                .NotNull(x => x.SourceDto.Name)
                .Build();

        // Create a single DTO converter.
        static Method createSingleDtoConverter(Contracts.ViewModels.DtoViewModel src, TypePath srcClass, TypePath dstClass, string methodName) =>
            new Method(methodName)
            {
                IsExtension = true,
                Body = CodeConstants.Converter_ConvertSingle_MethodBody(srcClass.Name!, dstClass.Name!, "o", src.Properties.Select(x => x.Name)),
                ReturnType = dstClass
            }.AddParameter(srcClass.Name!, "o");

        // Create an enumerable converter.
        static Method createEnumerableConverter(TypePath srcClass, TypePath dstClass, string methodName) =>
            new Method(methodName)
            {
                IsExtension = true,
                Body = CodeConstants.Converter_ConvertEnumerable_MethodBody(srcClass.Name!, dstClass.Name!, "o"),
                ReturnType = $"System.IEnumerable<{dstClass.Name}>"
            }.AddParameter($"System.IEnumerable<{srcClass.Name}>", "o");

        // Create an extension class and add members.
        static Class createExtensionClassAndAddMembers(TypePath srcClass, Method ma, Method mb) =>
            new Class(srcClass.Name!) { InheritanceModifier = InheritanceModifier.Partial | InheritanceModifier.Static, AccessModifier = AccessModifier.Public }
                    .AddMember(ma)
                    .AddMember(mb);

        // Create a namespace.
        static INamespace createNameSpace(Contracts.ViewModels.DtoViewModel src) =>
            INamespace.New($"{src.NameSpace}.Converters");

        // Add the class to the namespace.
        static void AddClassToNameSpace(Class cl, INamespace ns) =>
            ns.AddType(cl);

        // Add `using` statements
        static void AddUsings(Contracts.ViewModels.DtoViewModel src, Method ma, Method mb, INamespace ns)
        {
            if (src.NameSpace != null)
            {
                _ = ns.AddUsingNameSpace(src.NameSpace);
            }

            if (ma.ReturnType?.NameSpace != null)
            {
                _ = ns.AddUsingNameSpace(ma.ReturnType.NameSpace);
            }

            if (mb.ReturnType?.NameSpace != null)
            {
                _ = ns.AddUsingNameSpace(mb.ReturnType.NameSpace);
            }

            _ = ns.AddUsingNameSpace(ma.Parameters.Select(x => TypePath.GetNameSpace(x.Type)));
            _ = ns.AddUsingNameSpace(mb.Parameters.Select(x => TypePath.GetNameSpace(x.Type)));
        }
    }
}