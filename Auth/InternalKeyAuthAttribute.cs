using GnosisKernel.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace GnosisKernel.Api.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class InternalKeyAuthAttribute : Attribute, IAsyncActionFilter
{
    private const string HeaderName = "X-Internal-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var opt = context.HttpContext.RequestServices.GetRequiredService<IOptions<InternalOptions>>().Value;

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var provided) ||
            string.IsNullOrWhiteSpace(opt.ApiKey) ||
            provided != opt.ApiKey)
        {
            context.Result = new UnauthorizedObjectResult("Invalid internal key.");
            return;
        }

        await next();
    }
}