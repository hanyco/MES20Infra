using Library.Cqrs.Models.Queries;

namespace Test.HumanResources.Dtos
{
    public sealed class GetByIdPersonQueryParams : IQuery<GetByIdPersonQueryResult>
    {
        public GetByIdPersonQueryParams(GetByIdPersonParams @params)
        {
            this.Params = @params;
        }

        public GetByIdPersonParams Params { get; set; }
    }
}