namespace Test.HumanResources.Dtos
{
    public sealed class GetByIdPersonQueryParams
    {
        public GetByIdPersonQueryParams(GetByIdPersonParams params)
        {
            this.Params =  params ; 
        }

        public GetByIdPersonParams Params { get; set; }
    }
}