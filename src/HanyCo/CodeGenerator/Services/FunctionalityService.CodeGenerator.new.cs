using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HanyCo.Infra.CodeGen.Contracts.CodeGen.Services;
using Library.CodeGeneration.Models;
using Library.Results;

namespace Services;
partial class FunctionalityService
{
    public Result<Codes?> GenerateCodes(FunctionalityViewModel model, FunctionalityCodeServiceAsyncCodeGeneratorArgs? arguments = null)
    {
        // Validate the model
        var validationResult = this.Validate(model);
        if (!validationResult.IsSucceed)
        {
            return validationResult.WithValue(Codes.Empty)!;
        }

        try
        {
            var allCodes = new List<Code>();

            //// Generate codes for Blazor Components
            //if (model.BlazorDetailsComponentViewModel is not null)
            //{
            //    var detailComponentCodes = this.GenerateBlazorComponentCode(model.BlazorDetailsComponentViewModel);
            //    model.Codes.BlazorDetailsComponentCodes = detailComponentCodes;
            //    allCodes.Add(detailComponentCodes);
            //}

            //// Generate codes for  Blazor Pages
            //if (model.BlazorDetailsPageViewModel is not null)
            //{
            //    var detailPageCodes = this.GenerateBlazorPageCode(model.BlazorDetailsPageViewModel);
            //    model.Codes.BlazorDetailsPageCodes = detailPageCodes;
            //    allCodes.Add(detailPageCodes);
            //}

            //// Generate codes for  CQRS Commands
            //if (model.InsertCommandViewModel is not null)
            //{
            //    var insertCommandCodes = this.GenerateCqrsCommandCode(model.InsertCommandViewModel);
            //    model.Codes.InsertCommandCodes = insertCommandCodes;
            //    allCodes.Add(insertCommandCodes);
            //}

            //// Add the other codes like DeleteCommand, UpdateCommand, GetByIdQuery, GetAllQuery, etc.
            //// Example Generate the other codes CQRS Delete Command
            //if (model.DeleteCommandViewModel is not null)
            //{
            //    var deleteCommandCodes = this.GenerateCqrsCommandCode(model.DeleteCommandViewModel);
            //    model.Codes.DeleteCommandCodes = deleteCommandCodes;
            //    allCodes.Add(deleteCommandCodes);
            //}

            // And so on...
            // ...

            // return the result
            var result = Codes.New(allCodes);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Fail<Codes>(ex.GetBaseException().Message);
        }
    }
}
