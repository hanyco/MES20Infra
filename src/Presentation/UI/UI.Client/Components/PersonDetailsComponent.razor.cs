namespace HumanResources.Dtos
{
    using HumanResource.Mapper;
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
                var @params = new HumanResource.Dtos.GetByIdPerson()
                {
                    Id = entityId,
                };
                var cqParams = new HumanResource.Dtos.GetByIdPersonQuery(@params);
                // Invoke the query handler to retrieve all entities
                var cqResult = await this._queryProcessor.ExecuteAsync<HumanResource.Dtos.GetByIdPersonQueryResult>(cqParams);
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