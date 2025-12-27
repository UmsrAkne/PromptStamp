using Prism.Mvvm;

namespace PromptStamp.Models
{
    public class DiffPrompt : BindableBase
    {
        private string key;
        private string prompt;
        private bool isEnabled;

        public DiffPrompt()
        {
            IsEnabled = true;
        }

        public string Key { get => key; set => SetProperty(ref key, value); }

        public string Prompt { get => prompt; set => SetProperty(ref prompt, value); }

        public bool IsEnabled { get => isEnabled; set => SetProperty(ref isEnabled, value); }

        public DiffPrompt Clone()
        {
            return new DiffPrompt()
            {
                Key = Key,
                Prompt = Prompt,
            };
        }
    }
}