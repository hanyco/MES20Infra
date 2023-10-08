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

    public Result<Codes> GenerateCodes(ModelConverterCodeParameter args)
    {
        if (!validate(args).TryParse(out var vr))
        {
            return vr.WithValue(Codes.Empty);
        }
        (var src, var srcClass, var dstClass, var methodName) = args;
        methodName ??= CodeConstants.Converter_Convert_MethodName(dstClass.Name!);

        var ma = new Method(methodName)
        {
            IsExtension = true,
            Body = CodeConstants.Converter_ConvertSingle_MethodBody(srcClass.Name!, dstClass.Name!, "o", src.Properties.Select(x => x.Name)),
            ReturnType = dstClass
        }.AddParameter(srcClass.Name!, "o");
        var mb = new Method(methodName)
        {
            IsExtension = true,
            Body = CodeConstants.Converter_ConvertEnumerable_MethodBody(srcClass.Name!, dstClass.Name!, "o"),
            ReturnType = $"System.IEnumerable<{dstClass.Name}>"
        }.AddParameter($"System.IEnumerable<{srcClass.Name}>", "o");

        var cl = new Class(srcClass.Name!) { InheritanceModifier = InheritanceModifier.Partial | InheritanceModifier.Static }
            .AddMember(ma)
            .AddMember(mb);

        var ns = INamespace.New($"{src.NameSpace}.Converters")
            .AddType(cl);

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

        var codeGenRes = this._codeGenerator.Generate(ns);
        if (codeGenRes.IsFailure)
        {
            return codeGenRes.WithValue(Codes.Empty);
        }
        var result = Code.New(methodName, Languages.CSharp, codeGenRes, true, $"ModelConverter.{srcClass}.{methodName}.cs")
            .With(x => x.props().Category = CodeCategory.Converter)
            .ToCodes();

        //var singleConverter = CodeConstants.Converter_ConvertSingle_MethodBody(srcClassName, dstClassName, "o", src.Properties.Select(x => x.Name));
        //var enumerableConverter = CodeConstants.Converter_ConvertEnumerable_MethodBody(srcClassName, dstClassName, "o");
        //var statement = CodeConstants.WrapInClass("ModelConverter", true, accessModifier: MemberAttributes.Public, singleConverter, enumerableConverter);
        //var result = Code.New(methodName, Languages.CSharp, statement, true, $"ModelConverter.{srcClassName}.{methodName}.cs")
        //.With(x => x.props().Category = CodeCategory.Converter)
        //.ToCodes();

        return Result<Codes>.CreateSuccess(result);

        [return: NotNull]
        static Result validate([NotNull] ModelConverterCodeParameter? viewModel) => viewModel.ArgumentNotNull()
                .Check()
                .NotNull(x => x.SourceDto)
                .NotNull(x => x.SourceDto.Name)
                .Build();
    }
}