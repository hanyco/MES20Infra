using Blazored.LocalStorage;

using Library.Results;

using Web.UI.Helpers;

namespace Web.UI.Services;

public sealed class ApiClientService(ILocalStorageService _localStorage, IHttpClientFactory httpClientFactory)
{
    private string? _cachedToken;

    public HttpClient CreateClient() => httpClientFactory.CreateClient("ApiClient");

    public async Task<string?> GetTokenAsync()
    {
        if (string.IsNullOrEmpty(this._cachedToken))
        {
            this._cachedToken = await _localStorage.GetItemAsync<string>("authToken");
        }
        return this._cachedToken;
    }

    public string? GetUserNameFromToken(string token)
    {
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == "full_name");
        if (claim?.Value?.IsNullOrEmpty() is not false)
        {
            claim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");
        }
        return claim?.Value;
    }

    public bool IsTokenExpired(string token)
    {
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var expiration = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

        if (expiration != null && long.TryParse(expiration, out var expUnix))
        {
            var expirationDate = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            return expirationDate < DateTime.UtcNow;
        }
        return true;
    }

    public async Task LogoutAsync()
    {
        this._cachedToken = null;
        await _localStorage.RemoveItemAsync("authToken");
    }

    public async Task<Result<T?>> SendApiRequestAsync<T>(string apiUrl)
    {
        var token = await this.GetTokenAsync();
        using var httpClient = this.CreateClient();
        var result = await httpClient.SendApiRequestAsync<T>(token, apiUrl);
        return result;
    }

    public async Task<Result<HttpResponseMessage>> SendApiRequestWithoutResponseAsync(string apiUrl, HttpMethod method, object? content = null)
    {
        var token = await this.GetTokenAsync();
        using var httpClient = this.CreateClient();
        var result = await httpClient.SendApiRequestWithoutResponseAsync(token, apiUrl, method, content);
        return result;
    }

    public async Task SetTokenAsync(string token)
    {
        this._cachedToken = token;
        await _localStorage.SetItemAsync("authToken", token);
    }
}
