using System.Collections.ObjectModel;
using Prism.Mvvm;
using PromptStamp.Utils;

namespace PromptStamp.Models
{
    public class ImagePromptGroup : BindableBase
    {
        private string header;

        public string Header { get => header; set => SetProperty(ref header, value); }

        public ObservableCollection<string> ImagePaths { get; set; } = new ();

        public ObservableCollection<DiffPrompt> DiffPrompts { get; set; } = new ();

        public void ApplyDiffPrompt(string basePrompt)
        {
            var prompt = basePrompt;

            foreach (var diffPrompt in DiffPrompts)
            {
                prompt = prompt.Replace(diffPrompt.Key, diffPrompt.Prompt);
            }

            foreach (var path in ImagePaths)
            {
                MetadataWriter.Write(path, prompt);
            }
        }
    }
}