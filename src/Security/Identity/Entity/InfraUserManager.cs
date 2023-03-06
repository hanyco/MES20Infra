//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

//namespace HanyCo.Infra.Security.Identity;

//internal sealed class InfraUserManager : UserManager<InfraIdentityUser>
//{
//    public InfraUserManager(IUserStore<InfraIdentityUser> store,
//                            IOptions<IdentityOptions> optionsAccessor,
//                            IPasswordHasher<InfraIdentityUser> passwordHasher,
//                            IEnumerable<IUserValidator<InfraIdentityUser>> userValidators,
//                            IEnumerable<IPasswordValidator<InfraIdentityUser>> passwordValidators,
//                            ILookupNormalizer keyNormalizer,
//                            IdentityErrorDescriber errors,
//                            IServiceProvider services,
//                            ILogger<UserManager<InfraIdentityUser>> logger)
//        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)

//    {
//    }
//}