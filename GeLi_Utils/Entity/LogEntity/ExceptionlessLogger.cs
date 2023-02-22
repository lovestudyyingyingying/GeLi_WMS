using Exceptionless;
using Exceptionless.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Entity.LogEntity
{
    public class ExceptionLessLogger : ILogger
    {
        /// <summary>
            /// Trace
            /// </summary>
        public void Trace(string message, params string[] tags)
        {
            ExceptionlessClient.Default.CreateLog(message, LogLevel.Trace).AddTags(tags).Submit();
        }

        /// <summary>
            /// Debug
            /// </summary>
        public void Debug(string message, params string[] tags)
        {
            ExceptionlessClient.Default.CreateLog(message, LogLevel.Debug).AddTags(tags).Submit();
        }

        /// <summary>
            /// Info
            /// </summary>
        public void Info(string message, params string[] tags)
        {
            ExceptionlessClient.Default.CreateLog(message, LogLevel.Info).AddTags(tags).Submit();
        }

        /// <summary>
            /// Warn
            /// </summary>
        public void Warn(string message, params string[] tags)
        {
            ExceptionlessClient.Default.CreateLog(message, LogLevel.Warn).AddTags(tags).Submit();
        }

        /// <summary>
            /// Error
            /// </summary>
        public void Error(string message, params string[] tags)
        {
            ExceptionlessClient.Default.CreateLog(message, LogLevel.Error).AddTags(tags).Submit();
        }
    }
}
