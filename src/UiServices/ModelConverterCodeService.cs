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

    public Result<Codes> GenerateCodes(ModelConverterCodeParameter parameter)
    {
        var vr = validate(parameter);
        if (!vr)
        {
            return vr.WithValue(Codes.Empty);
        }
        var (src, srcClassName, dstClassName, methodName) = parameter!;
        methodName ??= CodeConstants.Converter_Convert_MethodName(dstClassName);

        var ma = new Method(methodName)
        {
            IsExtension = true,
            Body = CodeConstants.Converter_ConvertSingle_MethodBody(srcClassName, dstClassName, "o", src.Properties.Select(x => x.Name)),
            ReturnType = dstClassName
        }.AddParameter(srcClassName, "o");
        var mb = new Method(methodName)
        {
            IsExtension = true,
            Body = CodeConstants.Converter_ConvertEnumerable_MethodBody(srcClassName, dstClassName, "o"),
            ReturnType = $"System.IEnumerable<{TypePath.GetName(dstClassName)}>"
        }.AddParameter($"System.IEnumerable<{TypePath.GetName(srcClassName)}>", "o");

        var cl = new Class(srcClassName) { InheritanceModifier = InheritanceModifier.Partial | InheritanceModifier.Static }
            .AddMember(ma)
            .AddMember(mb);

        var ns = INamespace.New($"{src.NameSpace}.Converters")
            .AddType(cl);

        var statement = this._codeGenerator.Generate(ns);
        var result = Code.New(methodName, Languages.CSharp, statement, true, $"ModelConverter.{srcClassName}.{methodName}.cs")
            .With(x => x.props().Category = CodeCategory.Converter)
            .ToCodes();

        //var singleConverter = CodeConstants.Converter_ConvertSingle_MethodBody(srcClassName, dstClassName, "o", src.Properties.Select(x => x.Name));
        //var enumerableConverter = CodeConstants.Converter_ConvertEnumerable_MethodBody(srcClassName, dstClassName, "o");
        //var statement = CodeConstants.WrapInClass("ModelConverter", true, accessModifier: MemberAttributes.Public, singleConverter, enumerableConverter);
        //var result = Code.New(methodName, Languages.CSharp, statement, true, $"ModelConverter.{srcClassName}.{methodName}.cs")
        //.With(x => x.props().Category = CodeCategory.Converter)
        //.ToCodes();

        return Result<Codes>.CreateSuccess(result);

        static Result validate(ModelConverterCodeParameter? viewModel) => viewModel.ArgumentNotNull()
                .Check()
                .NotNull(x => x.SourceDto)
                .NotNull(x => x.SourceDto.Name)
                .Build();
    }
}