using HumanResources.Dtos;
using System.Collections.Generic;

namespace HumanResources.Mappers;
public static partial class ModelConverter
{
    public static PersonDto ToViewModel(this GetByIdPersonQueryResult model)
    {
        var result = new PersonDto
        {
            PersonDto = model.PersonDto,
        };
        return result;
    }

    public static List<PersonDto> ToViewModel(this IEnumerable<GetByIdPersonQueryResult> models)
    {
        return models.Select(ToViewModel).ToList();
    }
}