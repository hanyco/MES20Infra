using System;

namespace Test.HumanResources.Dtos;
public sealed class GetAllPeopleResult
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int Height { get; set; }
}