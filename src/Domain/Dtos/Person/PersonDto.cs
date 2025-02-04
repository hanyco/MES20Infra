using System;

namespace Mes.HumanResourcesManagement.Dtos;
public sealed class PersonDto
{
    public Int64 Id { get; set; }
    public String? FirstName { get; set; }
    public String LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Int32? Height { get; set; }
}