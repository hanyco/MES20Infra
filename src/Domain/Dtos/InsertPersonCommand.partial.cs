using Library.Cqrs.Models.Queries;
using HumanResources.Dtos;
using System;

namespace HumanResources.Dtos;
public sealed partial class InsertPersonCommand : IQuery<InsertPersonCommandResult>
{
    public String? FirstName { get; set; }
    public String LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Int32? Height { get; set; }

    public InsertPersonCommand(String? firstName, String lastName, DateTime dateOfBirth, Int32? height)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.DateOfBirth = dateOfBirth;
        this.Height = height;
    }
}