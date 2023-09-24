using System.CodeDom;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.Definitions;

using Library.CodeGeneration.Models;
using Library.Interfaces;
using Library.Results;
using Library.Validations;

namespace Services;

internal sealed class ModelConverterCodeService : IBusinessService, IModelConverterCodeService
{
    public Result<Codes> GenerateCode(DtoViewModel src, string srcClassName, string dstClassName, string methodName)
    {
        if (!src.Check().ArgumentNotNull().NotNull(x => x.Name).TryParse(out var vr))
        {
            return vr.WithValue(Codes.Empty);
        }

        var singleConverter = CodeConstants.Converter_ConvertSingleSource(srcClassName, dstClassName, "o", src.Properties.Select(x => x.Name));
        var enumerableConverter = CodeConstants.Converter_ConvertEnumerableSource(srcClassName, dstClassName, "o");
        var statement = CodeConstants.WrapInClass("ModelConverter", true, accessModifier: MemberAttributes.Public, singleConverter, enumerableConverter);

        var result = Code.New(methodName, Languages.CSharp, statement, true, $"ModelConverter.{srcClassName}.{methodName}.cs")
            .With(x => x.props().Category = CodeCategory.Converter)
            .ToCodes();

        return Result<Codes>.CreateSuccess(result);
    }
}