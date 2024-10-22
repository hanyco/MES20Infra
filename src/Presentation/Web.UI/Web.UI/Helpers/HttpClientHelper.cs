using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components;

using Newtonsoft.Json.Linq;

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Web.UI.Helpers;

public static class HttpClientHelper
{
    public static async Task<T?> SendApiRequestAsync<T>(this HttpClient httpClient, ILocalStorageService localStorage, NavigationManager navigationManager, string apiUrl)
    {
        try
        {
            await SetAuthorization(httpClient, localStorage);

            // Send the API request
            var apiResult = await httpClient.GetFromJsonAsync<T>(apiUrl);
            return apiResult;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Redirect to login page if Unauthorized
            navigationManager.NavigateTo("/login");
            return default;
        }
        catch (Exception ex)
        {
            // Handle other errors
            Console.WriteLine($"Error: {ex.Message}");
            return default;
        }
    }

    public static async Task SetAuthorization(this HttpClient httpClient, ILocalStorageService localStorage)
    {
        // Get JWT token from local storage
        var tokenTask = localStorage.GetItemAsync<string>("authToken").AsTask();
        string? token;
        if (await Task.WhenAny(tokenTask, Task.Delay(5000)) == tokenTask)
        {
            token = await tokenTask;
        }
        else
        {
            token = null;
        }


        // If token is not empty, add it to the request header
        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public static async Task<HttpResponseMessage> SendApiRequestWithoutResponseAsync(this HttpClient httpClient, ILocalStorageService localStorage, NavigationManager navigationManager, string apiUrl, HttpMethod method, object? content = null)
    {
        try
        {
            await SetAuthorization(httpClient, localStorage);

            var request = new HttpRequestMessage(method, apiUrl);

            if (content is not null)
            {
                request.Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
            }

            var response = await httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                navigationManager.NavigateTo("/login");
            }

            return response;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Redirect to login page if Unauthorized
            navigationManager.NavigateTo("/login");
            return default;
        }
        catch (Exception ex)
        {
            // مدیریت سایر خطاها
            Console.WriteLine($"Error: {ex.Message}");
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}