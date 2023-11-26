using HumanResources.Dtos;
using HumanResources;
using System.Collections.Generic;
using System;

namespace HumanResources.Mapper;
public static partial class ModelConverter
{
    public static UpdatePerson ToUpdatePerson(this PersonDto model)
    {
        var result = new UpdatePerson
        {
            Id = model.Id,
        };
        return result;
    }

    public static List<UpdatePerson> ToUpdatePerson(this IEnumerable<PersonDto> models)
    {
        return models.Select(ToUpdatePerson).ToList();
    }
}