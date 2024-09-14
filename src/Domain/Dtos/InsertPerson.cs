using System;

namespace HumanResource.Dtos;
public sealed class InsertPerson
{
    public String? FirstName { get; set; }
    public String LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Int32? Height { get; set; }

    public InsertPerson(String? firstName, String lastName, DateTime dateOfBirth, Int32? height)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.DateOfBirth = dateOfBirth;
        this.Height = height;
    }
}