using Library.Cqrs.Models.Queries;

namespace Test.HumanResources.Dtos
{
    public sealed class GetAllPeopleQueryParams : IQuery<GetAllPeopleQueryResult>
    {
        public GetAllPeopleQueryParams(GetAllPeopleParams @params)
        {
            this.Params = @params;
        }

        public GetAllPeopleParams Params { get; set; }
    }
}