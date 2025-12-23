using System;

namespace PromptStamp.Utils.Log
{
    public record LogEntry(DateTime Timestamp, string Level, string Message);
}