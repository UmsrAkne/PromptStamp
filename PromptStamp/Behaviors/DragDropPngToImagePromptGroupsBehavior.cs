using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using PromptStamp.Factories;
using PromptStamp.Models;
using PromptStamp.ViewModels;

namespace PromptStamp.Behaviors
{
    public class DragDropPngToImagePromptGroupsBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject == null)
            {
                return;
            }

            AssociatedObject.AllowDrop = true;
            AssociatedObject.PreviewDragOver += OnPreviewDragOver;
            AssociatedObject.Drop += OnDrop;
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.PreviewDragOver -= OnPreviewDragOver;
                AssociatedObject.Drop -= OnDrop;
            }

            base.OnDetaching();
        }

        private void OnPreviewDragOver(object sender, DragEventArgs e)
        {
            if (HasPngFiles(e))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private static bool HasPngFiles(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return false;
            }

            if (e.Data.GetData(DataFormats.FileDrop) is not string[] files)
            {
                return false;
            }

            return files.Any(f => IsPng(f));
        }

        private static bool IsPng(string path)
        {
            try
            {
                return string.Equals(Path.GetExtension(path), ".png", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            if (e.Data.GetData(DataFormats.FileDrop) is not string[] files)
            {
                return;
            }

            var pngFiles = files.Where(IsPng).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (pngFiles.Count == 0)
            {
                return;
            }

            var groups = new List<ImagePromptGroup>();
            foreach (var file in pngFiles)
            {
                var ig = ImagePromptGroupFactory.Create();
                ig.Header = Path.GetFileNameWithoutExtension(file);
                ig.ImagePaths = new ObservableCollection<string> { file, };

                groups.Add(ig);
            }

            if (AssociatedObject?.DataContext is MainWindowViewModel vm)
            {
                vm.PromptGroupListViewModel.Items = new ObservableCollection<ImagePromptGroup>(groups);
            }
        }
    }
}