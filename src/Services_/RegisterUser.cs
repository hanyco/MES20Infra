
//namespace HanyCo.Infra.Security.Users
//{
//    using System;
//    using System.Linq;
//    using System.Threading.Tasks;

//    using HanyCo.Infra.Core.Identity.Entities;
//    using HanyCo.Infra.Web.Exceptions;

//    public sealed partial class RegisterUserCommandHandler
//    {

//        public async Task<RegisterUserCommandResult> HandleAsync(RegisterUserCommandParameter parameter)
//        {
//            var user = new InfraIdentityUser
//            {
//                UserName = parameter.Dto.UserName,
//                Email = parameter.Dto.Email,
//                EmailConfirmed = true
//            };
//            var idnResult = await this.UserManager.CreateAsync(user, parameter.Dto.Password);
//            if (idnResult.Succeeded)
//            {
//                return new RegisterUserCommandResult(Guid.Empty);
//            }
//            var errorMessage = idnResult.Errors.Select(err => $"({err.Code}) {err.Description}").Merge(Environment.NewLine);
//            throw new CannotCreareException(errorMessage);
//        }
//    }
//}
