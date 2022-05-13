using CQRS_MediatR.API.Controllers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using NuGet.Protocol;

namespace CQRS_MediatR.API.Filters;

public class LoggingAttribute : Attribute, IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        var _logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();

        string request = "Request:";

        List<string> requests = new List<string>
        {
            "User-Agent: " + context.HttpContext.Request.Headers.UserAgent,
            "Protocol: " + context.HttpContext.Request.Protocol,
            "Cookie: " + context.HttpContext.Request.Headers.Cookie,
            "Method: " + context.HttpContext.Request.Method,
            "Path: " + context.HttpContext.Request.Path,
            "ContentType: " + context.HttpContext.Request.ContentType
        };

        foreach (var elem in requests)
        {
            request += $"\n\t{elem}";
        }

         _logger.LogInformation(request);
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        var _logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();

        string response = "Response:";

        List<string> responses = new List<string>
        {
            "User-Agent: " + context.HttpContext.Response.Headers.UserAgent,
            "Cookie: " + context.HttpContext.Response.Headers["access_token"],
            "StatusCode: " + context.HttpContext.Response.StatusCode
        };

        foreach (var elem in responses)
        {
            response += $"\n\t{elem}";
        }

        _logger.LogInformation(response);
    }
}