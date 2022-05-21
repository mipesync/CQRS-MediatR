namespace CQRS_MediatR.API.Logging
{
    public class CombinedLogger : ILogger
    {
        public void LogInfo(string message)
        {
            ICollection<ILogger> loggers = new List<ILogger>() { new ConsoleLogger(), new FileLogger() };

            foreach (var logger in loggers)
            {
                logger.LogInfo(message);
            }
        }
    }
}
