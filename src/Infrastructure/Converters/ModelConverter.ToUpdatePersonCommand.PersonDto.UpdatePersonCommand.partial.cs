using HumanResources.Dtos;
using System.Collections.Generic;
using System;

namespace HumanResources.Mappers;
public static partial class ModelConverter
{
    public static UpdatePersonCommand ToUpdatePersonCommand(this PersonDto model)
    {
        var result = new UpdatePersonCommand
        {
            Id = model.Id,
        };
        return result;
    }

    public static List<UpdatePersonCommand> ToUpdatePersonCommand(this IEnumerable<PersonDto> models)
    {
        return models.Select(ToUpdatePersonCommand).ToList();
    }
}