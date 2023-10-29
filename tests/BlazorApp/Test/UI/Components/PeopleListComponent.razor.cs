
namespace Test.HumanResources
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    
    
    public sealed partial class PeopleListComponent
    {
        
        public void Delete(long id)
        {
        }
        
        protected override Task OnLoadAsync()
        {
            return Task.CompletedTask;
        }
    }
}
