using System;

namespace Test.HumanResources.Dtos;
public sealed class InsertPerson
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int Height { get; set; }
}