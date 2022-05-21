using CQRS_MediatR.API.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Text.Json;
using System.Text.RegularExpressions;
using ILogger = CQRS_MediatR.API.Logging.ILogger;

namespace CQRS_MediatR.API.Filters;

public class LoggingAttribute : Attribute, IResultFilter
{
    private IServiceProvider _serviceProvider = null!;
    private ILogger _logger = null!;

    public void OnResultExecuting(ResultExecutingContext context)
    {
        _serviceProvider = context.HttpContext.RequestServices.GetRequiredService<IServiceProvider>();
        _logger = GetLogger(context.HttpContext.Request.Method)!;

        var request = context.HttpContext.Request;

        string requestStr = "Request:";

        List<string> requests = new List<string>
            {
                "User-Agent: " + request.Headers.UserAgent,
                "Protocol: " + request.Protocol,
                "Cookie: " + request.Headers.Cookie,
                "Method: " + request.Method,
                "Path: " + request.Path,
                "ContentType: " + request.ContentType,
                "Date: " + DateTime.Now.ToString()
            };

        foreach (var elem in requests)
        {
            requestStr += $"\n\t{elem}";
        }

        _logger.LogInfo(requestStr);

    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        _serviceProvider = context.HttpContext.RequestServices.GetRequiredService<IServiceProvider>();
        _logger = GetLogger(context.HttpContext.Request.Method)!;

        var response = context.HttpContext.Response;

        string responseStr = "Response:";

        string? body = null;
        try
        {
            body = context.Result.ToJson();
        }
        catch (JsonSerializationException) { body = "{ message:Deserialize error }"; }

        List<string> responses = new List<string>
        {
            "Cookie: " + response.Headers["access_token"],
            "StatusCode: " + (response.StatusCode == 302 ? 200 : response.StatusCode),
            "Body: " + ResultDeserialize(body),
            "Date: " + DateTime.Now.ToString()
        };

        foreach (var elem in responses)
        {
            responseStr += $"\n\t{elem}";
        }

        _logger.LogInfo(responseStr);
    }

    private static string ResultDeserialize(string? json)
    {
        if (json is null) return json!;

        var resultContent = json.Substring(2, json.Length - 4).Replace("\"", "").Split(",");
        var responseBody = "";

        foreach (var elem in resultContent)
        {
            if (Regex.IsMatch(elem, "message") || Regex.IsMatch(elem, "Url:") || Regex.IsMatch(elem, "ViewName"))
                responseBody = elem.Substring(elem.IndexOf(':') + 1);
        }
        return responseBody;
    }

    private ILogger? GetLogger(string method)
    {
        if (method == "GET")
            return _serviceProvider.GetService(typeof(ConsoleLogger)) as ILogger;
        else
            return _serviceProvider.GetService(typeof(FileLogger)) as ILogger;
    }
}