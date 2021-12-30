using Microsoft.Extensions.Logging;

namespace EF.GeneratedSelectBug.Tests.Logger;

public class SqlTranslationTests
{
    public class TestLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
            => new TestLogger();

        public void Dispose() { }
    }
}