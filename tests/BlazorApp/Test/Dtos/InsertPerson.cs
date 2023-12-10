using System;
using System.ComponentModel.DataAnnotations;

namespace HumanResources.Dtos;
public sealed class InsertPerson
{
    public String? FirstName { get; set; }

    [RequiredAttribute]
    public String LastName { get; set; }

    [RequiredAttribute]
    public DateTime DateOfBirth { get; set; }
    public Int32? Height { get; set; }
}