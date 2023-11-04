namespace Test.HumanResources
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed partial class PersonDetailsComponent
    {
        protected override async Task OnLoadAsync()
        {
            if (this.EntityId is { } entityId)
            {
                // Setup segregation parameters
                var @params = new GetByIdPersonParams()
                {
                    Id = entityId,
                };
                var cqParams = new Test.HumanResources.Dtos.GetByIdPersonQueryParams(@params);
                // Invoke the query handler to retrieve all entities
                var cqResult = await this._queryProcessor.ExecuteAsync<Test.HumanResources.Dtos.GetByIdPersonQueryResult>(cqParams);
                // Now, set the data context.
                this.DataContext = cqResult.Result.ToViewModel();
            }
            else
            {
                this.DataContext = new();
            }
        }

        private void ValidateForm()
        {
        }
    }
}