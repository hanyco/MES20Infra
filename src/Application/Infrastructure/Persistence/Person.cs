using System;
using System.Collections.Generic;

namespace Application.Infrastructure.Persistence;

public partial class Person
{
    public long Id { get; set; }

    public string? FirstName { get; set; }

    public string LastName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public int? Height { get; set; }
}
