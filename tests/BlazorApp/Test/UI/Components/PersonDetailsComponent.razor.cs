namespace Test.HumanResources
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed partial class PersonDetailsComponent
    {
        protected override Task OnLoadAsync()
        {
            return Task.CompletedTask;
        }

        private void ValidateForm()
        {
        }

        void OnInsertPersonCommandCalling(Test.HumanResources.Dtos.InsertPersonParams parameter)
        {
        }

        void OnInsertPersonCommandCalled(Test.HumanResources.Dtos.InsertPersonParams parameter, Test.HumanResources.Dtos.InsertPersonResult result)
        {
        }
    }
}