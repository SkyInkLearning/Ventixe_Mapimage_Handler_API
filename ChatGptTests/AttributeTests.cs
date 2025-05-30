using Application.Extensions.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatGptTests;

public class AttributeTests
{
    private ActionExecutingContext CreateContext(string headerKey, string headerValue, string configuredApiKey)
    {
        var services = new ServiceCollection();
        var configDict = new Dictionary<string, string> { { "ApiKeys:StandardApiKey", configuredApiKey } };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
        services.AddSingleton(configuration);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };

        if (headerKey != null)
            httpContext.Request.Headers[headerKey] = headerValue;

        var actionContext = new ActionContext
        {
            HttpContext = httpContext,
            RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
            ActionDescriptor = new ControllerActionDescriptor()
        };

        return new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object>(),
            controller: null
        );
    }

    [Fact]
    public async Task OnActionExecutionAsync_SetsUnauthorized_WhenHeaderMissing()
    {
        var filter = new UseApiKeyAttribute();
        var context = CreateContext(null, null, "secret-key");
        bool nextCalled = false;
        Task<ActionExecutedContext> Next() { nextCalled = true; return Task.FromResult<ActionExecutedContext>(null); }

        await filter.OnActionExecutionAsync(context, Next);

        Assert.False(nextCalled);
        var result = Assert.IsType<UnauthorizedObjectResult>(context.Result);

        // reflect into the anonymous payload
        var payload = result.Value!;
        var t = payload.GetType();
        var success = (bool)t.GetProperty("success")!.GetValue(payload)!;
        var error = (string)t.GetProperty("error")!.GetValue(payload)!;

        Assert.False(success);
        Assert.Equal("Invalid api-key or api-key is missing.", error);
    }


    [Fact]
    public async Task OnActionExecutionAsync_CallsNext_WhenKeyValid()
    {
        var filter = new UseApiKeyAttribute();
        var context = CreateContext("X-API-KEY", "valid-key", "valid-key");
        bool nextCalled = false;
        Task<ActionExecutedContext> Next() { nextCalled = true; return Task.FromResult<ActionExecutedContext>(null); }

        await filter.OnActionExecutionAsync(context, Next);

        Assert.True(nextCalled);
        Assert.Null(context.Result);
    }
}
