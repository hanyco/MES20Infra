using System.CodeDom;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.Definitions;

using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
using Library.Helpers.CodeGen;
using Library.Interfaces;
using Library.Results;
using Library.Validations;

namespace Services;

internal sealed class ModelConverterCodeService(ICodeGeneratorEngine codeGenerator) : IBusinessService, IModelConverterCodeService
{
    private readonly ICodeGeneratorEngine _codeGenerator = codeGenerator;

    public Result<Codes> GenerateCodes(ModelConverterCodeViewModel viewModel, GenerateCodesParameters? arguments = null)
    {
        if (!viewModel.SourceDto.Check().ArgumentNotNull().NotNull(x => x.Name).TryParse(out var vr))
        {
            return vr.WithValue(Codes.Empty);
        }
        (var src, var srcClassName, var dstClassName, var methodName) = viewModel;
        methodName ??= CodeConstants.Converter_Convert_MethodName(dstClassName);
        var ns = INamespace.New($"{src.NameSpace}.Converters");
        var cl = new Class(srcClassName) { InheritanceModifier = InheritanceModifier.Partial | InheritanceModifier.Static };
        var ma = new Method(CodeConstants.Converter_Convert_MethodName(dstClassName)) { IsExtension = true };
        cl.Members.Add(ma);

        var rs = _codeGenerator.Generate(ns);
        
        var singleConverter = CodeConstants.Converter_ConvertSingle_MethodBody(srcClassName, dstClassName, "o", src.Properties.Select(x => x.Name));
        var enumerableConverter = CodeConstants.Converter_ConvertEnumerable_MethodBody(srcClassName, dstClassName, "o");
        var statement = CodeConstants.WrapInClass("ModelConverter", true, accessModifier: MemberAttributes.Public, singleConverter, enumerableConverter);
        var result = Code.New(methodName, Languages.CSharp, statement, true, $"ModelConverter.{srcClassName}.{methodName}.cs")
        .With(x => x.props().Category = CodeCategory.Converter)
        .ToCodes();

        return Result<Codes>.CreateSuccess(result);
    }
}