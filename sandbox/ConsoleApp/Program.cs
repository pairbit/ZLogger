using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using ZLogger;

using var factory = LoggerFactory.Create(logging =>
{
    // Add ZLogger provider to ILoggingBuilder
    logging.AddZLoggerConsole(options =>
    {
        options.UsePlainTextFormatter(formatter =>
        {
             formatter.SetPrefixFormatter($"foo", (in MessageTemplate template, in LogInfo info) => template.Format());
            //MessageTemplate
            // LogInfo
            //formatter.SetPrefixFormatter($"foo", (template, info) => template.Format());
        });
    });

    //// Output Structured Logging, setup options
    //logging.AddZLoggerConsole(options => options.UseJsonFormatter(formatter =>
    //{
    //    formatter.IncludeProperties = IncludeProperties.ParameterKeyValues | IncludeProperties.MemberName | IncludeProperties.FilePath | IncludeProperties.LineNumber;
    //}));

    //logging.AddZLoggerConsole(options =>
    //{
    //    options.InternalErrorLogger = ex => Console.WriteLine(ex);
    //    options.UseFormatter(() => new CLEFMessageTemplateFormatter());
    //});


});






var logger = factory.CreateLogger("Program");

var name = "John";
var age = 33;


//logger.LogInformation("aiueo {id:0,0000} ", 100);

LogLog.Foo3(logger, "a", "b", 100, new { aaaaa = 1000 });

LogLog.Foo(logger, "tako", "huga", 1000);

public static partial class LogLog
{
    [ZLoggerMessage(LogLevel.Information, "foo is {name} {city} {age:-1,0000}")]
    public static partial void Foo(ILogger logger, string name, string city, int age, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = 0);


    [ZLoggerMessage(LogLevel.Information, "foo is {name} {city} {age}")]
    public static partial void Foo2(ILogger logger, string name, string city, int age);


    [ZLoggerMessage(LogLevel.Information, "foo is {name} {city} {age:-1,0000}")]
    public static partial void Foo3(ILogger logger, string name, string city, int age, [ZLoggerContext] object? context = null, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = 0);
}