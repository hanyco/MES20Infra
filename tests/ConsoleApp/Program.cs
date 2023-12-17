using Library.CodeGeneration;
using Library.Data.SqlServer.Builders;
using Library.Helpers;

namespace ConsoleApp;

internal class Program
{
    private static void Main(string[] args)
    {
        var id = new WhereClauseCreatorField("Id", TypePath.New<long>());
        var fName = new WhereClauseCreatorField("FirstName", TypePath.New<string?>());
        var lName = new WhereClauseCreatorField("LastName", TypePath.New<string?>());
        var birthDate = new WhereClauseCreatorField("BirthDate", TypePath.New<DateOnly>());
        var height = new WhereClauseCreatorField("Height", TypePath.New<int>());
        var fields = new HashSet<WhereClauseCreatorField>([id, fName, lName, birthDate, height]);

        var idBiggerThan2 = new WhereClauseCreatorOperation(id, WhereClauseCreatorFieldOperator.IsBiggerThan, [2]);
        var idLessThan10 = new WhereClauseCreatorOperation(id, WhereClauseCreatorFieldOperator.IsLessThan, [10]);
        var nameIsNotNull = new WhereClauseCreatorOperation(fName, WhereClauseCreatorFieldOperator.IsNotNull);
        var nameContainsAli = new WhereClauseCreatorOperation(fName, WhereClauseCreatorFieldOperator.Contains, ["Ali"]);
        var lastEndsWithZadeh = new WhereClauseCreatorOperation(lName, WhereClauseCreatorFieldOperator.EndsWith, ["Zadeh"]);
        var lastIsNull = new WhereClauseCreatorOperation(fName, WhereClauseCreatorFieldOperator.IsNull);
        var birthDateEqualsDate = new WhereClauseCreatorOperation(birthDate, WhereClauseCreatorFieldOperator.EndsWith, [DateOnly.Parse("10/12/1977")]);

        var model = WhereClauseCreatorModel.New()
            .AddOperation(idBiggerThan2)
            .AddOperation(idLessThan10)
            .AddOperation(nameContainsAli)
            .AddOperation(lastEndsWithZadeh)
            .AddOperation(birthDateEqualsDate)
            .AddOperation(lastIsNull)
            .AddOperation(nameIsNotNull);

        var code = WhereClauseCreator.GenerateCode(model).ThrowOnFail().Value;
        Console.WriteLine(code);
    }
}