using Auth0.AspNetCore.Authentication;
using Dapr.Client;
using DaprApiPoc;
using DaprApiPoc.Components;
using DaprApiPoc.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Services.Models.Weather;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<TokenHandler>();

builder.Services.AddHttpClient();

builder.Services
    .AddHttpClient("ExternalAPI", client => client.BaseAddress = new Uri(builder.Configuration["ExternalApiBaseUrl"]))
    .AddHttpMessageHandler<TokenHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ExternalAPI"));

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddDaprClient();

builder.Services.AddAuth0WebAppAuthentication(opt =>
    {
        opt.Domain = Environment.GetEnvironmentVariable("daprpoc_auth0_domain");
        opt.ClientId = Environment.GetEnvironmentVariable("daprpoc_auth0_clientid");
        opt.ClientSecret = Environment.GetEnvironmentVariable("daprpoc_auth0_clientsecret");
    })
    .WithAccessToken(opt =>
    {
        opt.Audience = Environment.GetEnvironmentVariable("daprpoc_auth0_audience");
    });

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/api/externalData", async (DaprClient daprClient) =>
{
    var req = daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "api-feature", "WeatherForecast");
    req.AddQueryParameters(new Dictionary<string, string>
    {
        { "c", "2" }
    });
    var response = await daprClient.InvokeMethodAsync<WeatherForecast[]>(req);
    return response;
});

app.MapGet("/api/externalDataAuth", async (DaprClient daprClient) =>
{
    var req = daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "api-feature", "WeatherForecast/auth");
    req.AddQueryParameters(new Dictionary<string, string>
    {
        { "c", "2" }
    });
    var response = await daprClient.InvokeMethodAsync<WeatherForecast[]>(req);
    return response;
});

app.MapGet("/Account/Login", async (HttpContext HttpContext, string redirectUri = "/") =>
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(redirectUri)
            .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    })
    .AllowAnonymous();

app.Map("/Account/Logout", async (HttpContext httpContext, string redirectUri = "/") =>
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(redirectUri)
            .Build();

        await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    })
    .AllowAnonymous();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(DaprApiPoc.Client._Imports).Assembly);

app.Run();
