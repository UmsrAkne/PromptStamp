using Prism.Mvvm;

namespace PromptStamp.Models
{
    public class ImagePromptGroup : BindableBase
    {
        private string header;

        public string Header { get => header; set => SetProperty(ref header, value); }
    }
}