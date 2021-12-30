using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace EF.GeneratedSelectBug.Tests.Logger;

public class TestLogger : ILogger
{
    public static List<string> Logs { get; set; } = new List<string>();

    private static readonly object _lock = new object();

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        lock (_lock)
            Logs.Add(formatter(state, exception));
    }

    public bool IsEnabled(LogLevel logLevel)
        => true;

    public IDisposable BeginScope<TState>(TState state)
        => throw new NotImplementedException();
}