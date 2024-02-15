using Microsoft.AspNetCore.Builder;
using Infrastructure.Adapters;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureServices
{
    public static WebApplicationBuilder AddInfrastructureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient("adapterClient", c => c.BaseAddress = new Uri("http://localhost:6001/api"));
        builder.Services.AddTransient<IApiAdapter, ApiAdapter>();
        return builder;
    }
}