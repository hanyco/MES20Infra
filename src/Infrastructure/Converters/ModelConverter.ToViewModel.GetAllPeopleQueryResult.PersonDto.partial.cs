using HumanResources.Dtos;

using System.Collections.Generic;

namespace HumanResources.Mappers;
public static partial class ModelConverter
{
    public static PersonDto ToViewModel(this GetAllPeopleQueryResult model)
    {
        var result = new PersonDto
        {
            People = model.People,
        };
        return result;
    }

    public static List<PersonDto> ToViewModel(this IEnumerable<GetAllPeopleQueryResult> models)
    {
        return models.Select(ToViewModel).ToList();
    }
}