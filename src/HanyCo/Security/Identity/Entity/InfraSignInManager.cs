﻿using HanyCo.Infra.Security.Model;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HanyCo.Infra.Security.Identity;

public sealed class InfraSignInManager(UserManager<InfraIdentityUser> userManager,
                          IHttpContextAccessor contextAccessor,
                          IUserClaimsPrincipalFactory<InfraIdentityUser> claimsFactory,
                          IOptions<IdentityOptions> optionsAccessor,
                          ILogger<SignInManager<InfraIdentityUser>> logger,
                          IAuthenticationSchemeProvider schemes,
                          IUserConfirmation<InfraIdentityUser> confirmation) 
    : SignInManager<InfraIdentityUser>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
{
}