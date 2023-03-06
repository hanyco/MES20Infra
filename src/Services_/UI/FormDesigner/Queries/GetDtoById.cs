using System.Linq;
using System.Threading.Tasks;

using HanyCo.Infra.Dto;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Designers.UI.Queries
{
    public sealed partial class GetDtoByIdQueryHandler
    {

        public async Task<GetDtoByIdQueryResult> HandleAsync(GetDtoByIdQueryParameter parameter)
        {
            var query = from dto in this.WriteDbContext.Dtos
                        where dto.Guid == parameter.Id
                        select dto;
            var dbResult = await query.FirstOrDefaultAsync();
            var result = this.Mapper.Map<DTODto>(dbResult);
            return new GetDtoByIdQueryResult(result);
        }
    }
}
