﻿using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Text.Json;

namespace ZLog
{
    public readonly struct LogInfo
    {
        public readonly string CategoryName;
        public readonly DateTimeOffset Timestamp;
        public readonly LogLevel LogLevel;
        public readonly EventId EventId;
        public readonly Exception? Exception;
        public readonly bool IsJson;

        public LogInfo(string categoryName, DateTimeOffset timestamp, LogLevel logLevel, EventId eventId, Exception? exception, bool isJson)
        {
            EventId = eventId;
            CategoryName = categoryName;
            Timestamp = timestamp;
            LogLevel = logLevel;
            Exception = exception;
            IsJson = isJson;
        }

        static readonly JsonEncodedText CategoryNameText = JsonEncodedText.Encode(nameof(CategoryName));
        static readonly JsonEncodedText TimestampText = JsonEncodedText.Encode(nameof(Timestamp));
        static readonly JsonEncodedText LogLevelText = JsonEncodedText.Encode(nameof(LogLevel));
        static readonly JsonEncodedText EventIdText = JsonEncodedText.Encode(nameof(EventId));
        static readonly JsonEncodedText EventIdNameText = JsonEncodedText.Encode("EventIdName");
        static readonly JsonEncodedText ExceptionText = JsonEncodedText.Encode(nameof(Exception));

        static readonly JsonEncodedText Trace = JsonEncodedText.Encode(nameof(LogLevel.Trace));
        static readonly JsonEncodedText Debug = JsonEncodedText.Encode(nameof(LogLevel.Debug));
        static readonly JsonEncodedText Information = JsonEncodedText.Encode(nameof(LogLevel.Information));
        static readonly JsonEncodedText Warning = JsonEncodedText.Encode(nameof(LogLevel.Warning));
        static readonly JsonEncodedText Error = JsonEncodedText.Encode(nameof(LogLevel.Error));
        static readonly JsonEncodedText Critical = JsonEncodedText.Encode(nameof(LogLevel.Critical));
        static readonly JsonEncodedText None = JsonEncodedText.Encode(nameof(LogLevel.None));

        internal void WriteToJsonWriter(ref Utf8JsonWriter writer)
        {
            writer.WriteString(CategoryNameText, CategoryName);
            writer.WriteString(LogLevelText, LogLevelToEncodedText(LogLevel));
            writer.WriteNumber(EventIdText, EventId.Id);
            writer.WriteString(EventIdNameText, EventId.Name);
            writer.WriteString(TimestampText, Timestamp);
            writer.WritePropertyName(ExceptionText);
            WriteException(ref writer, Exception);
        }

        static void WriteException(ref Utf8JsonWriter writer, Exception? ex)
        {
            if (ex == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartObject();
                {
                    writer.WriteString("Name", ex.GetType().FullName);
                    writer.WriteString("Message", ex.Message);
                    writer.WriteString("StackTrace", ex.StackTrace);
                    writer.WritePropertyName("InnerException");
                    {
                        WriteException(ref writer, ex.InnerException);
                    }
                }
                writer.WriteEndObject();
            }
        }

        static JsonEncodedText LogLevelToEncodedText(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return Trace;
                case LogLevel.Debug:
                    return Debug;
                case LogLevel.Information:
                    return Information;
                case LogLevel.Warning:
                    return Warning;
                case LogLevel.Error:
                    return Error;
                case LogLevel.Critical:
                    return Critical;
                case LogLevel.None:
                    return None;
                default:
                    return JsonEncodedText.Encode(((int)logLevel).ToString());
            }
        }
    }

    public interface IZLogEntry
    {
        public LogInfo LogInfo { get; }
        void FormatUtf8(IBufferWriter<byte> writer, bool requireJavaScriptEncode);
        void Return();
    }
}
