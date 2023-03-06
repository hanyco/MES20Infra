using System.Linq;
using System.Threading.Tasks;

using HanyCo.Infra.Dto;
using HanyCo.Infra.Helpers;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Designers.UI.Queries
{
    public sealed partial class GetModulesQueryHandler
    {

        public async Task<GetModulesQueryResult> HandleAsync(GetModulesQueryParameter parameter)
        {
            var query = from module in this.WriteDbContext.Modules
                        select module;
            var dbResult = await query.ToListAsync();
            var result = this.Mapper.Map<ModuleDto>(dbResult);
            return new GetModulesQueryResult(result);
        }
    }
}
