using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}