namespace ggst_api.config
{
    public class CustomFileLoggerProvider : ILoggerProvider
    {

        private readonly CustomFileLoggerConfiguration _config;

        public CustomFileLoggerProvider(CustomFileLoggerConfiguration config)
        {
            this._config = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CustomFileLogger(categoryName, _config);
        }

        public void Dispose()
        {

        }
    }
}
