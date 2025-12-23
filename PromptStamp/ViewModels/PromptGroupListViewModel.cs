using System.Collections.ObjectModel;
using Prism.Mvvm;
using PromptStamp.Models;

namespace PromptStamp.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PromptGroupListViewModel : BindableBase
    {
        private ObservableCollection<ImagePromptGroup> items = new ();
        private ImagePromptGroup selectedItem;

        public ObservableCollection<ImagePromptGroup> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        public ImagePromptGroup SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
    }
}