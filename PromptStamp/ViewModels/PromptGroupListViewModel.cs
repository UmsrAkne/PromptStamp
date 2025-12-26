using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using PromptStamp.Models;
using PromptStamp.Utils.Log;

namespace PromptStamp.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PromptGroupListViewModel : BindableBase
    {
        private readonly IAppLogger logger;
        private ObservableCollection<ImagePromptGroup> items = new ();
        private ImagePromptGroup selectedItem;

        public PromptGroupListViewModel(IAppLogger logger)
        {
            this.logger = logger;
        }

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

        public DelegateCommand<DiffPrompt> RemoveDiffPromptCommand => new(prompt =>
        {
            if (prompt == null)
            {
                logger.Warn("RemoveDiffPromptCommand に null が渡されました。");
                return;
            }

            if (SelectedItem.DiffPrompts.Contains(prompt))
            {
                SelectedItem.DiffPrompts.Remove(prompt);
            }
        });
    }
}