using System;
using System.Collections.Generic;
using System.Linq;
using PromptStamp.Factories;
using PromptStamp.Models;
using PromptStamp.ViewModels;

namespace PromptStamp.Utils.Yaml
{
    public static class AppStateMapper
    {
        public static AppStateYaml ToDto(MainWindowViewModel vm)
        {
            if (vm == null)
            {
                throw new ArgumentNullException(nameof(vm));
            }

            if (vm.PromptGroupListViewModel == null)
            {
                throw new InvalidOperationException("PromptGroupListViewModel が null です。初期化漏れの可能性があります。");
            }

            var groups = vm.PromptGroupListViewModel.Items
                .Select(MapGroup)
                .ToList();

            return new AppStateYaml
            {
                CommonPrompt = vm.CommonPrompt,
                Groups = groups,
            };
        }

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

        private static ImagePromptGroupYaml MapGroup(ImagePromptGroup g)
        {
            if (g == null)
            {
                throw new ArgumentNullException(nameof(g));
            }

            return new ImagePromptGroupYaml
            {
                Header = g.Header,
                ImagePaths = g.ImagePaths?.ToList() ?? new List<string>(),
                DiffPrompts = g.DiffPrompts?.Select(MapDiff).ToList() ?? new List<DiffPromptYaml>(),
            };
        }

        private static DiffPromptYaml MapDiff(DiffPrompt d)
        {
            if (d == null)
            {
                throw new ArgumentNullException(nameof(d));
            }

            return new DiffPromptYaml
            {
                Key = d.Key,
                Prompt = d.Prompt,
                IsEnabled = d.IsEnabled,
            };
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