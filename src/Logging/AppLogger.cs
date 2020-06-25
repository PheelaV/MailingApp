using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailingApp.Logging
{
    class AppLogger : ILogger
    {
        TelemetryClient telemetryClient;

        public AppLogger(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        //TODO: https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-trace-logs
        /// <summary>
        /// Loging of exceptions
        /// </summary>
        /// <param name="exception">The exception thrown</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            throw new NotImplementedException();
        }

        public void LogException(Exception exception)
        {
            //Log into app insights
            this.telemetryClient.TrackException(exception);
            //Log into DB

            //Log into file
        }
    }
}
