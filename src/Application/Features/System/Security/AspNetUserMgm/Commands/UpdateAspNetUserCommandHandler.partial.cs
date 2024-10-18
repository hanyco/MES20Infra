using MediatR;
using Library.Data.SqlServer;
using System.Threading.Tasks;
using Mes.Security.Dtos;

namespace Mes.Security.Commands;
internal sealed partial class UpdateAspNetUserCommandHandler : IRequestHandler<UpdateAspNetUserCommand, UpdateAspNetUserCommandResult>
{
    private readonly IMediator _mediator;
    private readonly Sql _sql;
    public UpdateAspNetUserCommandHandler(IMediator mediator, Sql sql)
    {
        this._mediator = mediator;
        this._sql = sql;
    }

    public async Task<UpdateAspNetUserCommandResult> Handle(UpdateAspNetUserCommand request, CancellationToken cancellationToken)
    {
        var userName = request.AspNetUser.UserName?.ToString().IsNullOrEmpty() ?? true ? "null" : $"N'{request.AspNetUser.UserName.ToString()}'";
        var normalizedUserName = request.AspNetUser.NormalizedUserName?.ToString().IsNullOrEmpty() ?? true ? "null" : $"N'{request.AspNetUser.NormalizedUserName.ToString()}'";
        var email = request.AspNetUser.Email?.ToString().IsNullOrEmpty() ?? true ? "null" : $"N'{request.AspNetUser.Email.ToString()}'";
        var normalizedEmail = request.AspNetUser.NormalizedEmail?.ToString().IsNullOrEmpty() ?? true ? "null" : $"N'{request.AspNetUser.NormalizedEmail.ToString()}'";
        var emailConfirmed = $"N'{request.AspNetUser.EmailConfirmed.ToString()}'";
        var passwordHash = request.AspNetUser.PasswordHash?.ToString().IsNullOrEmpty() ?? true ? "null" : $"N'{request.AspNetUser.PasswordHash.ToString()}'";
        var securityStamp = request.AspNetUser.SecurityStamp?.ToString().IsNullOrEmpty() ?? true ? "null" : $"N'{request.AspNetUser.SecurityStamp.ToString()}'";
        var concurrencyStamp = request.AspNetUser.ConcurrencyStamp?.ToString().IsNullOrEmpty() ?? true ? "null" : $"N'{request.AspNetUser.ConcurrencyStamp.ToString()}'";
        var phoneNumber = request.AspNetUser.PhoneNumber?.ToString().IsNullOrEmpty() ?? true ? "null" : $"N'{request.AspNetUser.PhoneNumber.ToString()}'";
        var phoneNumberConfirmed = $"N'{request.AspNetUser.PhoneNumberConfirmed.ToString()}'";
        var twoFactorEnabled = $"N'{request.AspNetUser.TwoFactorEnabled.ToString()}'";
        var lockoutEnd = request.AspNetUser.LockoutEnd?.ToString().IsNullOrEmpty() ?? true ? "null" : $"N{SqlTypeHelper.FormatDate(request.AspNetUser.LockoutEnd)}"; ; 
        var lockoutEnabled = $"N'{request.AspNetUser.LockoutEnabled.ToString()}'";
        var accessFailedCount = request.AspNetUser.AccessFailedCount.ToString();
        var dbCommand = $@"UPDATE [dbo].[AspNetUsers]   SET [UserName] = {userName}, [NormalizedUserName] = {normalizedUserName}, [Email] = {email}, [NormalizedEmail] = {normalizedEmail}, [EmailConfirmed] = {emailConfirmed}, [PasswordHash] = {passwordHash}, [SecurityStamp] = {securityStamp}, [ConcurrencyStamp] = {concurrencyStamp}, [PhoneNumber] = {phoneNumber}, [PhoneNumberConfirmed] = {phoneNumberConfirmed}, [TwoFactorEnabled] = {twoFactorEnabled}, [LockoutEnd] = {lockoutEnd}, [LockoutEnabled] = {lockoutEnabled}, [AccessFailedCount] = {accessFailedCount}   WHERE [Id] = {request.Id}";
        var dbResult = await this._sql.ExecuteScalarCommandAsync(dbCommand, cancellationToken);
        var result = new UpdateAspNetUserCommandResult();
        return result;
    }
}