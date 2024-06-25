using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZLogger.Tests;

public class ScopeTest
{
    TestProcessor processor;
    ILogger logger;

    public ScopeTest()
    {
        var options = new ZLoggerOptions
        {
            IncludeScopes = true
        };
        options.UseJsonFormatter();
            
        processor = new TestProcessor(options);

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.IncludeScopes = true;
                return processor;
            });
        });
        logger = loggerFactory.CreateLogger("test");
    }
        
    [Fact]
    public void VersionMismatch()
    {
        var invalidScopeStateOwner = new InvalidScopeStateOwner();
        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.AddZLoggerLogProcessor(options =>
            {
                options.IncludeScopes = true;
                return invalidScopeStateOwner;
            });
        });
        logger = loggerFactory.CreateLogger("test");

        using (logger.BeginScope("X={X}", 123))
        {
            logger.LogInformation($"AAAAAAA");
        }
        using (logger.BeginScope("X={X}", 456))
        {
            logger.LogInformation($"BBBBBBBB");
        }

        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = invalidScopeStateOwner.LogInfos[0].ScopeState!.Properties;
        });
    }

    class InvalidScopeStateOwner : IAsyncLogProcessor
    {
        public List<LogInfo> LogInfos { get; private set; } = [];

        public ValueTask DisposeAsync() => default;

        public void Post(IZLoggerEntry entry)
        {
            LogInfos.Add(entry.LogInfo);
            entry.Return();
        }
    }
}
