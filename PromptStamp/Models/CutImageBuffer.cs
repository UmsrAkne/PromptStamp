using Prism.Mvvm;

namespace PromptStamp.Models
{
    public class CutImageBuffer : BindableBase
    {
        private string imagePath;
        private ImagePromptGroup sourceGroup;

        public string ImagePath
        {
            get => imagePath;
            set => SetProperty(ref imagePath, value);
        }

        public ImagePromptGroup SourceGroup
        {
            get => sourceGroup;
            set => SetProperty(ref sourceGroup, value);
        }

        public bool HasItem => ImagePath != null;
    }
}