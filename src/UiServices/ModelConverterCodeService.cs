using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.Definitions;

using Library.CodeGeneration.Models;
using Library.Interfaces;
using Library.Results;

namespace Services;

internal sealed class ModelConverterCodeService : IBusinessService, IModelConverterCodeService
{
    public Result<Codes> GenerateCode(DtoViewModel src, string dstClassName, string methodName)
    {
        var codeStatement = CodeConstants.ConverterToModelClassSource(src.Name, dstClassName, "viewModel", src.Properties.Select(x => x.Name));

        var codes = Code.New(methodName, Languages.CSharp, codeStatement, true, $"ModelConverter.{methodName}.cs")
            .With(x => x.props().Category = CodeCategory.Converter)
            .ToCodes();
        return Result<Codes>.CreateSuccess(codes);
    }
}

//public static partial class ModelConverter
//{
//    public static B ToInsertPersonParams(this A viewModel)
//    {
//        var result = new B
//        {
//            Name = viewModel.Name,
//            Age = viewModel.Age
//        };
//        return result;
//    }
//}

//public record A(string Name, int Age);
//public record B(string Name, int Age);