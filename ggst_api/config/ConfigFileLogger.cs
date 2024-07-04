using Microsoft.Extensions.Logging;
namespace ggst_api.config
{
    public class CustomFileLogger : ILogger
    {

        private readonly string _name;
        private readonly CustomFileLoggerConfiguration _config;
        private LogLevel _logLevel;

        public CustomFileLogger(string name, CustomFileLoggerConfiguration config)
        {
            _name = name;
            _config = config;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == _config.LogLevel;
            //return _config.MinLevel < logLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            _logLevel = logLevel;
            FileLog($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:fff")} - {logLevel.ToString()} - {_name} - {formatter(state, exception)}");
        }

        private async void FileLog(string strLog)
        {
            string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "_" + _logLevel.ToString() + ".txt";
            string filePath = _config.LogPath + "\\" + fileName;
            File.AppendAllText(filePath, strLog);
            await File.AppendAllTextAsync(filePath, strLog);
        }
    }
}
