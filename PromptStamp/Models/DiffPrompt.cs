using Prism.Mvvm;

namespace PromptStamp.Models
{
    public class DiffPrompt : BindableBase
    {
        private string key;
        private string prompt;

        public string Key { get => key; set => SetProperty(ref key, value); }

        public string Prompt { get => prompt; set => SetProperty(ref prompt, value); }

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