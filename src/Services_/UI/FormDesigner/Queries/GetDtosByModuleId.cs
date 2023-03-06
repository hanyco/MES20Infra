
namespace HanyCo.Infra.Designers.UI.Queries
{
    using System.Linq;
    using System.Threading.Tasks;

    using HanyCo.Infra.Dto;
    using HanyCo.Infra.Helpers;

    using Microsoft.EntityFrameworkCore;


    public sealed partial class GetDtosByModuleIdQueryHandler
    {

        public async Task<GetDtosByModuleIdQueryResult> HandleAsync(GetDtosByModuleIdQueryParameter parameter)
        {
            var query = from dto in this.WriteDbContext.Dtos
                        where dto.Module.Guid == parameter.Id
                        select dto;
            var dbResult = await query.ToListAsync();
            var result = this.Mapper.Map<DTODto>(dbResult);
            return new GetDtosByModuleIdQueryResult(result);
        }
    }
}
