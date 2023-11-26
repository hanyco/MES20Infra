using HumanResources.Dtos;
using HumanResources;
using System.Collections.Generic;
using System;

namespace HumanResources.Mapper;
public static partial class ModelConverter
{
    public static InsertPerson ToInsertPerson(this PersonDto model)
    {
        var result = new InsertPerson
        {
            Id = model.Id,
        };
        return result;
    }

    public static List<InsertPerson> ToInsertPerson(this IEnumerable<PersonDto> models)
    {
        return models.Select(ToInsertPerson).ToList();
    }
}