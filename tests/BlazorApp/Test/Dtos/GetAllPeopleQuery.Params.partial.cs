namespace Test.HumanResources.Dtos
{
    public sealed class GetAllPeopleQueryParams
    {
        public GetAllPeopleQueryParams(GetAllPeopleParams params)
        {
            this.Params =  params ; 
        }

        public GetAllPeopleParams Params { get; set; }
    }
}