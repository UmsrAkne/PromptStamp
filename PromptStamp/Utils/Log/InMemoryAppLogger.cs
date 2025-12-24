using System;
using System.Collections.ObjectModel;

namespace PromptStamp.Utils.Log
{
    public class InMemoryAppLogger : IAppLogger
    {
        public ObservableCollection<LogEntry> LogEntries { get; } = new ();

        public void Info(string message)
        {
            Add("INFO", message);
        }

        public void Warn(string message)
        {
            Add("WARN", message);
        }

        public void Error(string message, Exception ex = null)
        {
            Add("ERROR", ex is null ? message : $"{message}\n{ex}");
        }

        private void Add(string level, string message)
        {
            var entry = new LogEntry(DateTime.Now, level, message);
            LogEntries.Insert(0, entry);
            Console.WriteLine($"{entry.Timestamp} [{level}] {message}");
        }
    }
}