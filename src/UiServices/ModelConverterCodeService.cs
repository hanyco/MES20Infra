using System.Text;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

using Library.CodeGeneration.Models;
using Library.Interfaces;

namespace Services;

public sealed class ModelConverterCodeService : IBusinessService, IModelConverterCodeService
{
    public Code GenerateCode(DtoViewModel src, string dstClassName, string methodName)
    {
        var argName = "viewModel";
        var sourceBuffer = new StringBuilder();
        _ = sourceBuffer.AppendLine($"{HtmlDoc.INDENT.Repeat(0)}public partial class ModelConverter");
        _ = sourceBuffer.AppendLine($"{HtmlDoc.INDENT.Repeat(0)}{{");
        _ = sourceBuffer.AppendLine($"{HtmlDoc.INDENT.Repeat(1)}public static {dstClassName} To{dstClassName}(this {src.Name} {argName})");
        _ = sourceBuffer.AppendLine($"{HtmlDoc.INDENT.Repeat(1)}{{");
        _ = sourceBuffer.AppendLine($"{HtmlDoc.INDENT.Repeat(2)}var result = new {dstClassName}");
        _ = sourceBuffer.AppendLine($"{HtmlDoc.INDENT.Repeat(2)}{{");
        foreach (var property in src.Properties)
        {
            _ = sourceBuffer.AppendLine($"{HtmlDoc.INDENT.Repeat(3)}{property.Name} = {argName}.{property.Name}");
        }
        _ = sourceBuffer.AppendLine($"{HtmlDoc.INDENT.Repeat(2)}}};");
        _ = sourceBuffer.AppendLine($"{HtmlDoc.INDENT.Repeat(2)}return result;");
        _ = sourceBuffer.AppendLine($"{HtmlDoc.INDENT.Repeat(1)}}}");
        _ = sourceBuffer.AppendLine($"{HtmlDoc.INDENT.Repeat(0)}}}");

        return Code.ToCode(methodName, Languages.CSharp, sourceBuffer.ToString(), true, $"ModelConverter.{methodName}");
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