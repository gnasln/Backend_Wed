using WED_BACKEND_ASP.Infrastructure;

namespace WED_BACKEND_ASP.Endpoints;

public class HealthCheck : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup("HealthCheck").WithTags("Ping").MapGet("/ping", () =>
        {
            return 1;
        })
        .WithName("Ping")
        .WithOpenApi();
    }
}