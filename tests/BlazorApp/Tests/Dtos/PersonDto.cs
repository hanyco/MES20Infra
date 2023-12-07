using System;
using System.ComponentModel.DataAnnotations;

namespace HumanResources;
public sealed class PersonDto
{
    public Int64 Id { get; set; }
    public String? FirstName { get; set; }
    [Required]
    public String LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Int32? Height { get; set; }
}