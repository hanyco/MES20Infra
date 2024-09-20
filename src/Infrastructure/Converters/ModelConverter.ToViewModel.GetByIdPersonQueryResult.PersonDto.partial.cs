using HumanResources.Dtos;
using System.Collections.Generic;
using System;

namespace HumanResources.Mappers;
public static partial class ModelConverter
{
    public static PersonDto ToViewModel(this GetByIdPersonQueryResult model)
    {
        var result = new PersonDto
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            DateOfBirth = model.DateOfBirth,
            Height = model.Height,
            PersonDto = model.PersonDto,
        };
        return result;
    }

    public static List<PersonDto> ToViewModel(this IEnumerable<GetByIdPersonQueryResult> models)
    {
        return models.Select(ToViewModel).ToList();
    }
}