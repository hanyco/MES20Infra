﻿using HanyCo.Infra.CodeGeneration.Definitions;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
using Library.Results;
using Library.Validations;

using Services.Helpers;

namespace Services;

internal sealed class ApiCodingService(ICodeGeneratorEngine codeGeneratorEngine) : IApiCodeGenerator
{
    public Result<Codes> GenerateCodes(ApiCodingViewModel viewModel, ApiCodingArgs? arguments = null)
    {
        // Validation
        {
            var vr = viewModel.Check()
                         .ArgumentNotNull()
                         .NotNull(x => x.DtoType)
                         .NotNull(x => x.NameSpace)
                         .Build();
            if (vr.IsFailure)
            {
                return vr.WithValue(Codes.Empty);
            }
        }
        arguments ??= new();

        var modelType = CommonHelpers.Purify(viewModel.DtoType);
        var controller = new Class(viewModel.ControllerName ?? $"{modelType}Controller")
            .AddBaseType("Microsoft.AspNetCore.Mvc.ControllerBase")
            .AddAttribute("Microsoft.AspNetCore.Mvc.ApiController")
            .AddAttribute("Microsoft.AspNetCore.Mvc.Route", (null, viewModel.ControllerRoute ?? "[controller]"));

        if (arguments.GenerateGetAll)
        {
            var getAll = new Method("GetAll")
            {
                Body = viewModel.GetAllApi?.Body ?? CodeConstants.DefaultMethodBody,
                ReturnType = viewModel.GetAllApi?.ReturnType ?? TypePathHelper.WrapWithTask(TypePathHelper.WrapWithIEnumetable(viewModel.DtoType)),
            }
            .AddAttribute("Microsoft.AspNetCore.Mvc.HttpGet");

            _ = controller.AddMember(getAll);
        }
        if (arguments.GenerateGetById)
        {
            var getById = new Method("GetById")
            {
                Body = viewModel.GetById?.Body ?? CodeConstants.DefaultMethodBody,
                ReturnType = viewModel.GetById?.ReturnType ?? TypePathHelper.WrapWithTask(viewModel.DtoType),
            }
            .AddAttribute("Microsoft.AspNetCore.Mvc.HttpGet", [(null, "{Id}")]);

            _ = controller.AddMember(getById);
        }
        if (arguments.GeneratePost)
        {
            var post = new Method("Post")
            {
                Body = viewModel.Post?.Body ?? CodeConstants.DefaultMethodBody,
            }
            .AddParameter(viewModel.Post?.Parameters.ElementAtOrDefault(0).Type ?? viewModel.DtoType, viewModel.Post?.Parameters.ElementAtOrDefault(0).Name ?? "value")
            .AddAttribute("Microsoft.AspNetCore.Mvc.HttpPost");

            _ = controller.AddMember(post);
        }
        if (arguments.GeneratePut)
        {
            var put = new Method("Put")
            {
                Body = viewModel.Put?.Body ?? CodeConstants.DefaultMethodBody,
            }
            .AddParameter(viewModel.Put?.Parameters.ElementAtOrDefault(0).Type ?? "long", viewModel.Put?.Parameters.ElementAtOrDefault(0).Name ?? "id")
            .AddParameter(viewModel.Put?.Parameters.ElementAtOrDefault(1).Type ?? viewModel.DtoType, viewModel.Put?.Parameters.ElementAtOrDefault(1).Name ?? "value")
            .AddAttribute("Microsoft.AspNetCore.Mvc.HttpPut");

            _ = controller.AddMember(put);
        }
        if (arguments.GenerateDelete)
        {
            var delete = new Method("Delete")
            {
                Body = viewModel.Delete?.Body ?? CodeConstants.DefaultMethodBody,
            }
            .AddParameter(viewModel.Delete?.Parameters.ElementAtOrDefault(0).Type ?? "long", viewModel.Delete?.Parameters.ElementAtOrDefault(0).Name ?? "id")
            .AddAttribute("Microsoft.AspNetCore.Mvc.HttpDelete", [(null, "{Id}")]);

            _ = controller.AddMember(delete);
        }

        var ns = INamespace.New(viewModel.NameSpace).AddType(controller);
        var statement = codeGeneratorEngine.Generate(ns);
        var code = new Code(controller.Name, Languages.CSharp, statement.Value).With(x => x.props().Category = CodeCategory.Api);
        return code.ToCodes();
    }
}