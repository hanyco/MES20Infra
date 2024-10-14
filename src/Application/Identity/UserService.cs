using Domain.Identity;

using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Identity;
public interface IUserService
{
    Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
    Task<ApplicationUser?> FindByNameAsync(string username);
}

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<ApplicationUser?> FindByNameAsync(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }
}
