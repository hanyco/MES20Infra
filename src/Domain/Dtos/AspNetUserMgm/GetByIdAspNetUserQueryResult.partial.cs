namespace Mes.Infra.Security.Dtos;
public sealed partial class GetByIdAspNetUserQueryResult
{
    public AspNetUserDto AspNetUserDto { get; set; }

    public GetByIdAspNetUserQueryResult(AspNetUserDto aspNetUserDto)
    {
        this.AspNetUserDto = aspNetUserDto;
    }
}