using HumanResources.Dtos;
using System.Collections.Generic;
using System;

namespace HumanResources.Mapper;
public static partial class ModelConverter
{
    public static PersonDto ToViewModel(this GetByIdPersonResult model)
    {
        var result = new PersonDto
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            DateOfBirth = model.DateOfBirth,
            Height = model.Height,
        };
        return result;
    }

    public static List<PersonDto> ToViewModel(this IEnumerable<GetByIdPersonResult> models)
    {
        return models.Select(ToViewModel).ToList();
    }
}