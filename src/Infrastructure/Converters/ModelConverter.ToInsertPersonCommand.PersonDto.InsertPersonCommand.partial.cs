using HumanResources.Dtos;
using System.Collections.Generic;
using System;

namespace HumanResources.Mappers;
public static partial class ModelConverter
{
    public static InsertPersonCommand ToInsertPersonCommand(this PersonDto model)
    {
        var result = new InsertPersonCommand
        {
        };
        return result;
    }

    public static List<InsertPersonCommand> ToInsertPersonCommand(this IEnumerable<PersonDto> models)
    {
        return models.Select(ToInsertPersonCommand).ToList();
    }
}