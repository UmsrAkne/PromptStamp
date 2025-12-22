using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace PromptStamp.Models
{
    public class ImagePromptGroup : BindableBase
    {
        private string header;

        public string Header { get => header; set => SetProperty(ref header, value); }

        public ObservableCollection<string> ImagePaths { get; set; } = new ();
    }
}