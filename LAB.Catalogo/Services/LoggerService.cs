using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LAB.Catalogo.Services
{
    public interface ILoggerService
    {
        void Info(string message, int statusCode = 200, [CallerFilePath] string sourceFile = null, [CallerMemberName] string sourceMethod = null);
        void Warn(string message, int statusCode = 200, [CallerFilePath] string sourceFile = null, [CallerMemberName] string sourceMethod = null);
        void Error(string message, int statusCode = 500, [CallerFilePath] string sourceFile = null, [CallerMemberName] string sourceMethod = null);
        void Critical(string message, int statusCode = 503, [CallerFilePath] string sourceFile = null, [CallerMemberName] string sourceMethod = null);
    }

    public class LoggerService : ILoggerService
    {
        private readonly IHostEnvironment _env;
        private readonly IServiceBus<Log> _serviceBus;
        private readonly HttpRequest _request;

        public LoggerService(IHostEnvironment env, IServiceBus<Log> serviceBus, IHttpContextAccessor accessor)
        {
            _env = env;
            _serviceBus = serviceBus;
            _request = accessor.HttpContext.Request;
        }

        public void Critical(string message, int statusCode = 503, [CallerFilePath] string sourceFile = null, [CallerMemberName] string sourceMethod = null)
        {
            var log = CreateLogInstance(LogType.critical, message, sourceFile, sourceMethod, statusCode);
            _serviceBus.Publish(log);
        }

        public void Error(string message, int statusCode = 500, [CallerFilePath] string sourceFile = null, [CallerMemberName] string sourceMethod = null)
        {
            var log = CreateLogInstance(LogType.error, message, sourceFile, sourceMethod, statusCode);
            _serviceBus.Publish(log);
        }

        public void Info(string message, int statusCode = 200, [CallerFilePath] string sourceFile = null, [CallerMemberName] string sourceMethod = null)
        {
            var log = CreateLogInstance(LogType.info, message, sourceFile, sourceMethod, statusCode);
            _serviceBus.Publish(log);
        }

        public void Warn(string message, int statusCode = 200, [CallerFilePath] string sourceFile = null, [CallerMemberName] string sourceMethod = null)
        {
            var log = CreateLogInstance(LogType.warn, message, sourceFile, sourceMethod, statusCode);
            _serviceBus.Publish(log);
        }

        private Log CreateLogInstance(LogType type, string message, string sourceFile, string sourceMethod, int statusCode)
        {
            var method = $"[{Path.GetFileName(sourceFile)}].[{sourceMethod}]";
            var logs = new Dictionary<LogType, Log>
            {
                { LogType.info, new Info(message, _env.EnvironmentName, _env.ApplicationName, method, statusCode)},
                { LogType.warn, new Warn(message, _env.EnvironmentName, _env.ApplicationName, method, statusCode)},
                { LogType.error, new Error(message, _env.EnvironmentName, _env.ApplicationName, method, statusCode)},
                { LogType.critical, new Critical(message, _env.EnvironmentName, _env.ApplicationName, method, statusCode)},
            };

            var log = logs[type];
            log.BrowserType = _request?.Headers["User-Agent"].ToString();
            log.CorrelationId = _request.Headers["X-Correlation-ID"].ToString();
            return log;
        }
    }

    public enum LogType
    {
        info,
        warn,
        error,
        critical,
    }

    public abstract class Log
    {
        public Log(string message, string envName, string appName, string method, int statusCode)
        {
            Data = new DataLog(message, method);
            StatusCode = statusCode;
            Environment = envName;
            Application = appName;
        }

        public string CorrelationId { get; set; }
        public string Environment { get; set; }
        public int StatusCode { get; set; }
        public string Severity { get; set; }
        public string Application { get; set; }
        public string Service { get; private set; } = "LAB-Catalogo WEB";
        public DateTime EventTimeUTC { get; private set; } = DateTime.UtcNow;
        public string BrowserType { get; set; }
        public DataLog Data { get; set; }
    }

    public class DataLog
    {
        public DataLog(string message, string method)
        {
            Message = message;
            Method = method;
        }

        public string Message { get; set; }
        public string RunTime { get; private set; } = RuntimeInformation.FrameworkDescription;
        public string Method { get; set; }
    }

    public class Info : Log
    {
        public Info(string message, string envName, string appName, string method, int statusCode = 200)
            : base(message, envName, appName, method, statusCode)
        {
            Severity = "INFO";
        }
    }

    public class Warn : Log
    {
        public Warn(string message, string envName, string appName, string method, int statusCode = 200)
            : base(message, envName, appName, method, statusCode)
        {
            Severity = "WARN";
        }
    }

    public class Error : Log
    {
        public Error(string message, string envName, string appName, string method, int statusCode = 500)
            : base(message, envName, appName, method, statusCode)
        {
            Severity = "ERROR";
        }
    }

    public class Critical : Log
    {
        public Critical(string message, string envName, string appName, string method, int statusCode = 503)
            : base(message, envName, appName, method, statusCode)
        {
            Severity = "CRITICAL";
        }
    }

    public static class LoggerConfig
    {
        public static IServiceCollection AddLogger(this IServiceCollection services, IConfiguration config)
        {
            services.AddServiceBus(config);
            services.AddScoped<ILoggerService, LoggerService>();
            return services;
        }
    }
}