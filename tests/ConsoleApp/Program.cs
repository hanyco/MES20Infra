using ConsoleApp.AdvancedSearch;

using Library.CodeGeneration;

namespace ConsoleApp;

internal class Program
{
    private static void Main(string[] args)
    {
        var id = new AdvancedSearchField("Id", TypePath.New<long>());
        var fName = new AdvancedSearchField("FirstName", TypePath.New<string?>());
        var lName = new AdvancedSearchField("LastName", TypePath.New<string?>());
        var birthDate = new AdvancedSearchField("BirthDate", TypePath.New<DateOnly>());
        var height = new AdvancedSearchField("Height", TypePath.New<int>());
        var fields = new HashSet<AdvancedSearchField>([id, fName, lName, birthDate, height]);

        var idBiggerThan2 = new AdvancedSearchOperation(id, AdvancedSearchFieldOperator.IsBiggerThan, [2]);
        var idLessThan10 = new AdvancedSearchOperation(id, AdvancedSearchFieldOperator.IsLessThan, [10]);
        var nameIsNotNull = new AdvancedSearchOperation(fName, AdvancedSearchFieldOperator.IsNotNull);
        var nameContainsAli = new AdvancedSearchOperation(fName, AdvancedSearchFieldOperator.Contains, ["Ali"]);
        var lastEndsWithZadeh = new AdvancedSearchOperation(lName, AdvancedSearchFieldOperator.EndsWith, ["Zadeh"]);
        var lastIsNull = new AdvancedSearchOperation(fName, AdvancedSearchFieldOperator.IsNull);
        var birthDateEqualsDate = new AdvancedSearchOperation(birthDate, AdvancedSearchFieldOperator.EndsWith, [DateOnly.Parse("10/12/1977")]);

        var model = AdvancedSearchViewModel.New()
            .AddOperation(idBiggerThan2)
            .AddOperation(idLessThan10)
            .AddOperation(nameContainsAli)
            .AddOperation(lastEndsWithZadeh)
            .AddOperation(birthDateEqualsDate)
            .AddOperation(lastIsNull)
            .AddOperation(nameIsNotNull);

        var code = AdvancedSearchImp.GenerateCode(model);
        Console.WriteLine(code);
    }
}