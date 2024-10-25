using Library.Results;

using Microsoft.AspNetCore.Components;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Web.UI.Helpers;

public static class HttpClientHelper
{
    public static async Task<T?> GetResponse<T>(this Task<Result<(T? Response, string? _)>> processResult)
    {
        var result = await processResult.ConfigureAwait(false);
        return result.Value.Response;
    }
    public static async Task<HttpResponseMessage> GetResponse(this Task<Result<(HttpResponseMessage Response, string? _)>> processResult)
    {
        var result = await processResult.ConfigureAwait(false);
        return result.Value.Response;
    }
    public static async Task<Result<(T? Response, string? ErrorMessage)>> ProcessResult<T>(this Task<Result<T?>> responseResultTask, NavigationManager navigation)
    {
        var responseResult = await responseResultTask.ConfigureAwait(false);
        var response = responseResult.Value;
        string? errorMessage = null;

        if (responseResult?.IsFailure is true)
        {
            if (responseResult.Exception is HttpRequestException ex && ex is { StatusCode: HttpStatusCode.Unauthorized })
            {
                navigation.NavigateTo("/login");
                return Result.Fail<(T? response, string? ErrorMessage)>((response, errorMessage));
            }
            else
            {
                errorMessage = responseResult.Exception?.Message ?? "Unknown error occurred.";
                return Result.Fail<(T? response, string? ErrorMessage)>((response, errorMessage));
            }
        }
        return Result.Success<(T? response, string? ErrorMessage)>((response, errorMessage));
    }

    [return: NotNull]
    public static Task<Result<(HttpResponseMessage Response, string? ErrorMessage)>> ProcessResult(this Task<Result<HttpResponseMessage>> responseResultTask, NavigationManager navigation) =>
        ProcessResult<HttpResponseMessage>(responseResultTask!, navigation)!;

    public static async Task<Result<T?>> SendApiRequestAsync<T>(this HttpClient httpClient, string? token, string apiUrl)
    {
        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Send the API request
            var apiResult = await httpClient.GetFromJsonAsync<T>(apiUrl);
            return apiResult;
        }
        catch (HttpRequestException ex) when (ex is { StatusCode: HttpStatusCode.Unauthorized })
        {
            // Redirect to login page if Unauthorized
            return Result.Fail<T?>(ex);
        }
        catch (Exception ex)
        {
            // Handle other errors
            return Result.Fail<T?>(ex);
        }
    }

    public static async Task<Result<HttpResponseMessage>> SendApiRequestWithoutResponseAsync(this HttpClient httpClient, string? token, string apiUrl, HttpMethod method, object? content = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var request = new HttpRequestMessage(method, apiUrl);

            if (content is not null)
            {
                request.Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
            }

            var response = await httpClient.SendAsync(request);

            return Result.Fail<HttpResponseMessage>(new HttpRequestException(HttpRequestError.UserAuthenticationError, statusCode: HttpStatusCode.Unauthorized));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Redirect to login page if Unauthorized
            return Result.Fail<HttpResponseMessage>(ex);
        }
        catch (Exception ex)
        {
            // مدیریت سایر خطاها
            return Result.Fail<HttpResponseMessage>(ex);
        }
    }
}

public static class ApiResponseHelper
{
    public static async Task<(T ApiResult, bool IsValid, string? ErrorMessage)> HandleApiResponse<T>(this Task<Result<(T? Response, string? ErrorMessage)>> apiProcessResult, string? messageOnApiResultIsNull = null)
        where T : new()
    {
        var (isOk, (apiResult, errMessage)) = await apiProcessResult.ConfigureAwait(false);
        return (isOk, apiResult) switch
        {
            (false, _) => (new(), false, errMessage),
            (_, null) => (new(), false, messageOnApiResultIsNull),
            _ => (apiResult, true, null)
        };
    }
}
