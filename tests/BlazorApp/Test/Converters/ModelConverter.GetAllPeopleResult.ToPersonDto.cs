public static partial class ModelConverter
{
    public static PersonDto ToPersonDto(this GetAllPeopleResult o)
    {
        var result = new PersonDto
        {
            Id = o.Id
            FirstName = o.FirstName
            LastName = o.LastName
            DateOfBirth = o.DateOfBirth
            Height = o.Height
        };
        return result;
    }
    
    public static IEnumerable<PersonDto?> ToPersonDto(this IEnumerable<GetAllPeopleResult?> o) =>
        models.Select(ToDbEntity);
    
}
