using Microsoft.AspNetCore.Mvc.Filters;

namespace CQRS_MediatR.API.Logging
{
    public interface ILogger
    {
        void LogInfo(string message);
    }
}
