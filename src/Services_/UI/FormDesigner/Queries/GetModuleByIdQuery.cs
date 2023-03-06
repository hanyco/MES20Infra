
namespace HanyCo.Infra.Designers.UI.Queries
{
    using System.Linq;
    using System.Threading.Tasks;

    using HanyCo.Infra.Core.Web.Exceptions;
    using HanyCo.Infra.Dto;
    using HanyCo.Infra.Helpers;

    using Microsoft.EntityFrameworkCore;


    public sealed partial class GetModuleByIdQueryHandler
    {

        public async Task<GetModuleByIdQueryResult> HandleAsync(GetModuleByIdQueryParameter parameter)
        {
            var query = from module in this.WriteDbContext.Modules
                        where module.Guid == parameter.Id
                        select module;
            var dbResult = await query.FirstOrDefaultAsync();
            if (dbResult is null)
            {
                throw new ObjectNotFoundException();
            }

            var result = this.Mapper.Map<ModuleDto>(dbResult);
            return new GetModuleByIdQueryResult(result);
        }
    }
}
