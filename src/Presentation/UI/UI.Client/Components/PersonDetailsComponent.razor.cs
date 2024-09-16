namespace HumanResources
{
    using HumanResources.Mappers;
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
                var @params = new HumanResources.Dtos.GetByIdPerson()
                {
                    Id = entityId,
                };
                var cqParams = new HumanResources.Dtos.GetByIdPersonQuery(@params);
                // Invoke the query handler to retrieve all entities
                var cqResult = await this._queryProcessor.ExecuteAsync<HumanResources.Dtos.GetByIdPersonQueryResult>(cqParams);
                // Now, set the data context.
                this.DataContext = cqResult.Result.ToViewModel();
            }
            else
            {
                this.DataContext = new();
            }
        }
    }
}