namespace ggst_api.config
{
    public class CustomFileLoggerConfiguration
    {

        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// 输出日志的最低级别，低于这个级别的不做输出，默认为Debug
        /// </summary>
        public LogLevel MinLevel { get; set; }

        /// <summary>
        /// 日志文件路径
        /// </summary>
        public string LogPath { get; set; }

        public CustomFileLoggerConfiguration()
        {
            MinLevel = LogLevel.Debug;
        }

    }
}
