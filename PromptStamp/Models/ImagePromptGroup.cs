using System.Collections.ObjectModel;
using Prism.Mvvm;
using PromptStamp.Utils;
using PromptStamp.Utils.Log;

namespace PromptStamp.Models
{
    public class ImagePromptGroup : BindableBase
    {
        private string header;

        public ImagePromptGroup(IAppLogger appLogger)
        {
            Logger = appLogger;
        }

        public string Header { get => header; set => SetProperty(ref header, value); }

        public ObservableCollection<string> ImagePaths { get; set; } = new ();

        public ObservableCollection<DiffPrompt> DiffPrompts { get; set; } = new ();

        private IAppLogger Logger { get; }

        public void ApplyDiffPrompt(string basePrompt)
        {
            Logger.Info($"ApplyDiffPrompt start: Images={ImagePaths.Count}, Diffs={DiffPrompts.Count}");

            var prompt = basePrompt;

            foreach (var diffPrompt in DiffPrompts)
            {
                Logger.Info($"Replace '{diffPrompt.Key}' -> '{diffPrompt.Prompt}'");
                prompt = prompt.Replace(diffPrompt.Key, diffPrompt.Prompt);
            }

            foreach (var path in ImagePaths)
            {
                MetadataWriter.Write(path, prompt);
            }

            Logger.Info("ApplyDiffPrompt completed");
        }
    }
}