using HumanResources.Dtos;
using System.Collections.Generic;
using System;

namespace HumanResources.Mappers;
public static partial class ModelConverter
{
    public static UpdatePerson ToUpdatePerson(this PersonDto model)
    {
        var result = new UpdatePerson
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            DateOfBirth = model.DateOfBirth,
            Height = model.Height,
        };
        return result;
    }

    public static List<UpdatePerson> ToUpdatePerson(this IEnumerable<PersonDto> models)
    {
        return models.Select(ToUpdatePerson).ToList();
    }
}