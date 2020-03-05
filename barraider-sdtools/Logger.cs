using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    /// <summary>
    /// Tracing levels used for Logger
    /// </summary>
    public enum TracingLevel
    {
        /// <summary>
        /// Debug level
        /// </summary>
        DEBUG,

        /// <summary>
        /// Informational level
        /// </summary>
        INFO,

        /// <summary>
        /// Warning level
        /// </summary>
        WARN,

        /// <summary>
        /// Error level
        /// </summary>
        ERROR,

        /// <summary>
        /// Fatal (highest) level
        /// </summary>
        FATAL
    }

    /// <summary>
    /// Log4Net logger helper class
    /// </summary>
    public class Logger
    {
        private static Logger instance = null;
        private static readonly object objLock = new object();

        /// <summary>
        /// Returns singelton entry of Log4Net logger
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                lock (objLock)
                {
                    if (instance == null)
                    {
                        instance = new Logger();
                    }
                    return instance;
                }
            }
        }

		/// <summary>
		/// Set logging format. In addition to the standard layouts defined for NLog, ${real-time} ${elapsed-time} are available.
		/// </summary>
		/// <example>"https://nlog-project.org/config/?tab=layout-renderers"</example>
		public string Layout
		{
			get
			{
				LoggingConfiguration lc = NLog.LogManager.Configuration;
				return ((FileTarget)lc.LoggingRules[0].Targets[0]).Layout.ToString();
			}
			set
			{
				LoggingConfiguration lc = NLog.LogManager.Configuration;
				((FileTarget)lc.LoggingRules[0].Targets[0]).Layout = value;
			}
		}

        private readonly NLog.Logger log = null;
        private Logger()
        {
			LayoutRenderer.Register<LayoutRenderers.ElapsedTimeLayoutRenderer>("elapsed-time");
			LayoutRenderer.Register<LayoutRenderers.RealTimeLayoutRenderer>("real-time");
			var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "pluginlog.log", ArchiveEvery=NLog.Targets.FileArchivePeriod.Day, MaxArchiveFiles=10, ArchiveFileName="logs/log.{###}.log", ArchiveNumbering=NLog.Targets.ArchiveNumberingMode.Rolling};
			config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = config;
            log = LogManager.GetCurrentClassLogger();
            LogMessage(TracingLevel.DEBUG, "Logger Initialized");
        }

        /// <summary>
        /// Add message to log with a specific severity level. 
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="Message"></param>
        public void LogMessage(TracingLevel Level, string Message)
        {
            switch (Level)
            {
                case TracingLevel.DEBUG:
                    log.Debug(Message);
                    break;

                case TracingLevel.INFO:
                    log.Info(Message);
                    break;

                case TracingLevel.WARN:
                    log.Warn(Message);
                    break;

                case TracingLevel.ERROR:
                    log.Error(Message);
                    break;

                case TracingLevel.FATAL:
                    log.Fatal(Message);
                    break;
            }
        }
    }
}
