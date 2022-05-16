using CQRS_MediatR.API.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using NuGet.Protocol;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CQRS_MediatR.API.Filters;

public class LoggingAttribute : Attribute, IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        var _logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();

        var request = context.HttpContext.Request;

        string requestStr = "Request:";
        string bodyStr = "";

        using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
        {
            bodyStr = reader.ReadToEnd();
        }

        List<string> requests = new List<string>
        {
            "User-Agent: " + request.Headers.UserAgent,
            "Protocol: " + request.Protocol,
            "Cookie: " + request.Headers.Cookie,
            "Method: " + request.Method,
            "Path: " + request.Path,
            "ContentType: " + request.ContentType,
            "Body: " + bodyStr,
            "Date: " + DateTime.Now.ToString()
        };

        foreach (var elem in requests)
        {
            requestStr += $"\n\t{elem}";
        }

        _logger.LogInformation(requestStr);

        using (var writer = new StreamWriter("log.txt", true))
        {
            writer.Write($"----------------\n{request}\n");
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        var _logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();

        var response = context.HttpContext.Response;

        string responseStr = "Response:";

        List<string> responses = new List<string>
        {
            "Cookie: " + response.Headers["access_token"],
            "StatusCode: " + (response.StatusCode == 302 ? 200 : response.StatusCode),
            "Body: " + ResultDeserialize(context.Result.ToJson()),
            "Date: " + DateTime.Now.ToString()
        };

        foreach (var elem in responses)
        {
            responseStr += $"\n\t{elem}";
        }

        _logger.LogInformation(responseStr);

        using (var writer = new StreamWriter("log.txt", true))
        {
            writer.Write($"\n{response}\n----------------\n");
        }
    }

    private string ResultDeserialize(string json)
    {
        var resultContent = json.Substring(2, json.Length - 4).Replace("\"", "").Split(",");
        var responseBody = "";

        foreach (var elem in resultContent)
        {
            if (Regex.IsMatch(elem, "message"))
                responseBody = elem.Substring(6);
            else if (Regex.IsMatch(elem, "Url"))
                responseBody = elem.Substring(4);
        }
        return responseBody;
    }
}