using Blazored.LocalStorage;

using Library.Results;

using Microsoft.AspNetCore.Components;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using Web.UI.Helpers;

namespace Web.UI.Services;

public sealed class ApiClientService(ILocalStorageService localStorage, IHttpClientFactory httpClientFactory)
{
    private string? _cachedToken;


    public HttpClient CreateClient()
    {
        return httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<string?> GetTokenAsync()
    {
        if (string.IsNullOrEmpty(_cachedToken))
        {
            _cachedToken = await localStorage.GetItemAsync<string>("authToken");
        }
        return _cachedToken;
    }

    public async Task SetTokenAsync(string token)
    {
        _cachedToken = token;
        await localStorage.SetItemAsync("authToken", token);
    }

    public void ClearToken()
    {
        _cachedToken = null;
    }


    public async Task<Result<T?>> SendApiRequestAsync<T>(string apiUrl)
    {
        var token = await GetTokenAsync();
        using var httpClient = CreateClient();
        var result = await httpClient.SendApiRequestAsync<T>(token, apiUrl);
        return result;
    }

    public async Task<Result<HttpResponseMessage>> SendApiRequestWithoutResponseAsync(string apiUrl, HttpMethod method, object? content = null)
    {
        var token = await GetTokenAsync();
        using var httpClient = CreateClient();
        return await httpClient.SendApiRequestWithoutResponseAsync(token, apiUrl, method, content);
    }
}
