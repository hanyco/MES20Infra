using Microsoft.AspNetCore.Components;
using System.Net;

public class CustomAuthorizationMessageHandler : DelegatingHandler
{
    private readonly NavigationManager _navigationManager;

    public CustomAuthorizationMessageHandler(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateTo("login");
        }

        return response;
    }
}