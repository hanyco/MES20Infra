using System;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed partial class GetByIdPersonQueryResult
{
    public Int64 Id { get; set; }
    public String? FirstName { get; set; }
    public String LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Int32? Height { get; set; }
    public PersonDto PersonDto { get; set; }

    public GetByIdPersonQueryResult(Int64 id, String? firstName, String lastName, DateTime dateOfBirth, Int32? height, PersonDto personDto)
    {
        this.Id = id;
        this.FirstName = firstName;
        this.LastName = lastName;
        this.DateOfBirth = dateOfBirth;
        this.Height = height;
        this.PersonDto = personDto;
    }
}