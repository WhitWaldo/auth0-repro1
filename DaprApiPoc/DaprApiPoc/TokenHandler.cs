using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace DaprApiPoc;

public class TokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var accessToken = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return await base.SendAsync(request, cancellationToken);
    }
}