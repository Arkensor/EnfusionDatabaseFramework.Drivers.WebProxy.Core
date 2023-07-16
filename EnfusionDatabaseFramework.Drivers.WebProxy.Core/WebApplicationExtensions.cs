using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace EnfusionDatabaseFramework.Drivers.WebProxy.Core;

public static class WebApplicationExtensions
{
    public static void AddDbProxy(this WebApplication app, [StringSyntax("Route")] string baseRoute = "")
    {
        baseRoute = baseRoute.TrimEnd('/');

        app.UseWhen(context => context.Request.Path.StartsWithSegments(baseRoute), branch =>
        {
            // Content type fix
            branch.Use(async (context, next) =>
            {
                // Modify content type as long as the rest client in enscript can't set it properly
                string userAgent = context.Request.Headers["User-Agent"].FirstOrDefault()?.ToLower() ?? string.Empty;
                if (userAgent.StartsWith("arma reforger"))
                {
                    context.Request.Headers["Content-Type"] = "application/json";
                }

                await next(context);
            });

            // Handle exceptions for all proxy endpoints
            branch.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {
                    context.Response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;

                    if (context.Features.Get<IExceptionHandlerFeature>()?.Error is ProxyRequestException proxyRequestException)
                    {
                        context.Response.StatusCode = proxyRequestException.Code;
                        await context.Response.WriteAsync($"{{\"statusCode\":{context.Response.StatusCode},\"error\":{proxyRequestException.Message}}}");
                        return;
                    }

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync($"{{\"statusCode\":{context.Response.StatusCode},\"error\":\"An unknown error has occurred.\"}}");
                });
            });
        });

        // Even though operations are async do not stop when AR is dying (it can on shutdown but we want to do the flush till end)
        var group = app.MapGroup(baseRoute + "/{database}/{collection}");
        group.MapPut("/{id}", HandleAddOrUpdateAsync);
        group.MapDelete("/{id}", HandleRemoveAsync);
        group.MapPost("", HandleFindAllAsync);
    }

    static async Task HandleAddOrUpdateAsync(HttpRequest request, string database, string collection, Guid id, IDbWebProxyService proxyService, CancellationToken cancellationToken)
    {
        using StreamReader stream = new(request.Body);
        string requestBody = await stream.ReadToEndAsync(cancellationToken);
        await proxyService.AddOrUpdateAsync(database, collection, id, requestBody, cancellationToken);
    }

    static async Task HandleRemoveAsync(HttpRequest request, string database, string collection, Guid id, IDbWebProxyService proxyService, CancellationToken cancellationToken)
    {
        using StreamReader stream = new(request.Body);
        string requestBody = await stream.ReadToEndAsync(cancellationToken);
        await proxyService.RemoveAsync(database, collection, id, cancellationToken);
    }

    static async Task<string> HandleFindAllAsync(HttpRequest request, string database, string collection, IDbWebProxyService proxyService, CancellationToken cancellationToken)
    {
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        var condition = await request.ReadFromJsonAsync<DbFindRequest>(options, cancellationToken);
        ArgumentNullException.ThrowIfNull(condition);

        // Put each result on one line until the enscript side is able to deserialize polymorph arrays
        var results = await proxyService.FindAllAsync(database, collection, condition, cancellationToken);
        return $"[\n{string.Join("\n", results)}\n]";
    }
}
