using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.RegularExpressions;

namespace CQRS_MediatR.API.Logging
{
    public class FileLogger : ILogger
    {
        public void LogInfo(string message)
        {
            using (var writer = new StreamWriter("log.txt", true))
            {
                writer.Write($"----------------\n{message}\n");
            }
        }
    }
}
