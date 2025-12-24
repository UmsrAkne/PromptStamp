using System;
using System.Collections.ObjectModel;

namespace PromptStamp.Utils.Log
{
    public interface IAppLogger
    {
        public ObservableCollection<LogEntry> LogEntries { get; }

        void Info(string message);

        void Warn(string message);

        void Error(string message, Exception ex = null);
    }
}