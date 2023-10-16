using System.Diagnostics.CodeAnalysis;

using Contracts.Services;
using Contracts.ViewModels;

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
        if (!validate(args).TryParse(out var vr))
        {
            return vr.WithValue(Codes.Empty);
        }

        (var sourceViewModel, var srcClass, var dstClass, var isSrvEnumerable) = args;

        var methods = new List<Method>();
        if (!isSrvEnumerable)
            methods.AddRange(createNormalMethods(sourceViewModel, srcClass, dstClass));
        var cl = createExtensionClassAndAddMembers(srcClass, dstClass, methods);
        var ns = createNameSpace(sourceViewModel);

        AddClassToNameSpace(cl, ns);
        AddUsings(ns, methods, sourceViewModel);

        var codeGenRes = this._codeGenerator.Generate(ns);
        if (codeGenRes.IsFailure)
        {
            return codeGenRes.WithValue(Codes.Empty);
        }

        var result = Code.New($"ModelConverter.{srcClass}", Languages.CSharp, codeGenRes, true, $"{srcClass}.ModelConverter.cs")
            .With(x => x.props().Category = CodeCategory.Converter)
            .ToCodes();
        return Result<Codes>.CreateSuccess(result);

        // Validate the input parameters.
        [return: NotNull]
        static Result validate([NotNull] ModelConverterCodeParameter viewModel) => viewModel.ArgumentNotNull()
                .Check()
                .NotNull(x => x.SourceDto)
                .NotNull(x => x.SourceDto.Name)
                .Build();

        // Create a single DTO converter.
        static Method createSingleDtoConverter(DtoViewModel src, TypePath srcClass, TypePath dstClass, string methodName) =>
            new Method(methodName)
            {
                IsExtension = true,
                Body = CodeConstants.Converter_ConvertSingle_MethodBody(srcClass, dstClass.Name, "o", src.Properties.Select(x => x.Name)),
                ReturnType = dstClass
            }.AddParameter(srcClass, "o");

        // Create an enumerable converter.
        static Method createEnumerableConverter(TypePath srcClass, TypePath dstClass, string methodName) =>
            new Method(methodName)
            {
                IsExtension = true,
                Body = CodeConstants.Converter_ConvertEnumerable_MethodBody(srcClass, dstClass, "o"),
                ReturnType = $"System.IEnumerable<{dstClass}>"
            }.AddParameter($"System.IEnumerable<{srcClass}>", "o");

        // Create an extension class and add members.
        static Class createExtensionClassAndAddMembers(TypePath srcClass, TypePath dstClass, IEnumerable<Method> methods)
        {
            var cl = new Class($"{srcClass.Name}To{dstClass.Name}Converter") { InheritanceModifier = InheritanceModifier.Partial | InheritanceModifier.Static };
            methods.ForEach(x => cl.Members.Add(x));
            return cl;
        }

        // Create a namespace.
        static INamespace createNameSpace(DtoViewModel src) =>
            INamespace.New($"{src.NameSpace}.Converters");

        // Add the class to the namespace.
        static void AddClassToNameSpace(Class cl, INamespace ns) =>
            ns.AddType(cl);

        // Add `using` statements
        static void AddUsings(INamespace ns, IEnumerable<Method> methods, DtoViewModel src)
        {
            if (src.NameSpace != null)
            {
                _ = ns.AddUsingNameSpace(src.NameSpace);
            }
            methods.ForEach(x => ns.AddUsingNameSpace(x.GetNameSpaces()));
        }

        static IEnumerable<Method> createNormalMethods(DtoViewModel dto, TypePath srcClass, TypePath dstClass)
        {
            var methodName = CodeConstants.Converter_Convert_MethodName(dstClass.Name);

            var ma = createSingleDtoConverter(dto, srcClass, dstClass, methodName);
            yield return ma;

            var mb = createEnumerableConverter(srcClass, dstClass, methodName);
            yield return mb;
        }
    }
}