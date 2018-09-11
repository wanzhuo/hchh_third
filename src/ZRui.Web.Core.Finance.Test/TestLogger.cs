using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Finance.Test
{
    public class TestLogger<T> : ILogger<T>
    {
        public TestLogger()
        {
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            
        }

        public void LogInformation(string message, params object[] args)
        {

        }
    }

    public class TestLoggerFactory:ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider)
        {
        }

        public TestLogger<T> CreateLogger<T>()
        {
            return new TestLogger<T>();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger<Member>();
        }

        public void Dispose()
        {
        }
    }
}
