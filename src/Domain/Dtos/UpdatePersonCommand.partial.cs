using Library.Cqrs.Models.Queries;
using HumanResources.Dtos;
using System;

namespace HumanResources.Dtos;
public sealed partial class UpdatePersonCommand : IQuery<UpdatePersonCommandResult>
{
    public Int64 Id { get; set; }
    public String? FirstName { get; set; }
    public String LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Int32? Height { get; set; }

    public UpdatePersonCommand(Int64 id, String? firstName, String lastName, DateTime dateOfBirth, Int32? height)
    {
        this.Id = id;
        this.FirstName = firstName;
        this.LastName = lastName;
        this.DateOfBirth = dateOfBirth;
        this.Height = height;
    }
}