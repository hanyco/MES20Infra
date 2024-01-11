using System.ComponentModel.DataAnnotations;

using HanyCo.Infra.Security.Exceptions;

using Microsoft.AspNetCore.Components;

namespace BlazorApp.Authentication;

public partial class LoginPage : ComponentBase
{
    private bool _isProcessing;
    public string ErrorMessage { get; set; }
    private LoginPageViewModel DataContext { get; set; } = new();

    private async Task SaveDataAsync()
    {
        if (this._isProcessing)
        {
            return;
        }

        this._isProcessing = true;
        try
        {
            var loginResult = await this._security.LogInAsync(this.DataContext.UserName, this.DataContext.Password, this.DataContext.IsPersist);
            if (loginResult.IsSucceed)
            {
                this.Nav.NavigateTo(string.Empty, forceLoad: false);
            }
            else
            {
                if (loginResult.Exception is NoUserFoundException)
                {
                    this.Nav.NavigateTo("/authentication/user");
                    return;
                }
                else
                {
                    this.ErrorMessage = loginResult.Errors.Select(x => x.Error.ToString()).Merge();
                }
            }
        }
        finally
        {
            this._isProcessing = false;
        }

        await Task.CompletedTask;
    }

    private sealed class LoginPageViewModel
    {
        public bool IsPersist { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}