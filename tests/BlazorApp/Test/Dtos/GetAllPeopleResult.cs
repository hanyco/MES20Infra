using System;
using System;
using System;
using System;
using System;

namespace Test.HumanResources.Dtos
{
    public sealed class GetAllPeopleResult
    {
        public Int64 Id { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Int32 Height { get; set; }
    }
}