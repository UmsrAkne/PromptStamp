using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using PromptStamp.Factories;
using PromptStamp.Models;
using PromptStamp.Utils.Log;
using PromptStamp.Utils.Yaml;

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

        public static void ApplyToViewModel(AppStateYaml dto, MainWindowViewModel vm)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (vm == null)
            {
                throw new ArgumentNullException(nameof(vm));
            }

            vm.CommonPrompt = dto.CommonPrompt ?? string.Empty;

            var list = vm.PromptGroupListViewModel;
            list.Items.Clear();

            foreach (var g in dto.Groups ?? Enumerable.Empty<ImagePromptGroupYaml>())
            {
                list.Items.Add(MapGroupToVm(g));
            }
        }

        private static ImagePromptGroup MapGroupToVm(ImagePromptGroupYaml g)
        {
            var group = ImagePromptGroupFactory.Create();
            group.Header = g.Header;

            if (g.ImagePaths != null)
            {
                foreach (var path in g.ImagePaths)
                {
                    group.ImagePaths.Add(path);
                }
            }

            if (g.DiffPrompts != null)
            {
                foreach (var d in g.DiffPrompts)
                {
                    group.DiffPrompts.Add(new DiffPrompt
                    {
                        Key = d.Key,
                        Prompt = d.Prompt,
                        IsEnabled = d.IsEnabled,
                    });
                }
            }

            return group;
        }
    }
}