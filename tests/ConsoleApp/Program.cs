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
        var nameContainsAli = new AdvancedSearchOperation(fName, AdvancedSearchFieldOperator.Contains, ["Ali"]);
        var lastEndsWithZadeh = new AdvancedSearchOperation(lName, AdvancedSearchFieldOperator.EndsWith, ["Zadeh"]);

        var model = new AdvancedSearchViewModel();
        _ = model.Operations.Add(idBiggerThan2);
        _ = model.Operations.Add(idLessThan10);
        _ = model.Operations.Add(nameContainsAli);
        _ = model.Operations.Add(lastEndsWithZadeh);

        var code = AdvancedSearchImp.GenerateCode(model);
        Console.WriteLine(code);
    }
}