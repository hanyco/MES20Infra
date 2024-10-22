using Blazored.LocalStorage;

namespace Web.UI.Services;

public sealed class TokenService
{
    private readonly ILocalStorageService _localStorage;
    private string? _cachedToken;

    public TokenService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<string?> GetTokenAsync()
    {
        if (string.IsNullOrEmpty(_cachedToken))
        {
            _cachedToken = await _localStorage.GetItemAsync<string>("authToken");
        }
        return _cachedToken;
    }

    public async Task SetTokenAsync(string token)
    {
        _cachedToken = token;
        await _localStorage.SetItemAsync("authToken", token);
    }

    public void ClearToken()
    {
        _cachedToken = null;
    }
}
