namespace Test.HumanResources.Dtos
{
    public sealed class GetAllPeopleQueryResult
    {
        public GetAllPeopleQueryResult(IEnumerable<GetAllPeopleResult> result)
        {
            this.Result = result;
        }

        public IEnumerable<GetAllPeopleResult> Result { get; set; }
    }
}