using HumanResources.Dtos;
using System.Collections.Generic;
using System;

namespace HumanResources.Mappers;
public static partial class ModelConverter
{
    public static InsertPerson ToInsertPerson(this PersonDto model)
    {
        var result = new InsertPerson
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            DateOfBirth = model.DateOfBirth,
            Height = model.Height,
        };
        return result;
    }

    public static List<InsertPerson> ToInsertPerson(this IEnumerable<PersonDto> models)
    {
        return models.Select(ToInsertPerson).ToList();
    }
}