using System;

namespace Test.HumanResources.Dtos
{
    public sealed class InsertPersonParams
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Height { get; set; }
    }
}