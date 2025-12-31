using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using PromptStamp.Models;

namespace PromptStamp.Behaviors
{
    /// <summary>
    /// ListBoxItem に対する PNG ドロップを受け付け、
    /// 既存 ImagePromptGroup に画像を追加する Behavior。
    /// </summary>
    public class AddImageToGroupBehavior : Behavior<StackPanel>
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

            return files.Any(IsPng);
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

            // ListBoxItem の DataContext が ImagePromptGroup
            if (AssociatedObject.DataContext is not ImagePromptGroup group)
            {
                return;
            }

            // 既存グループへ画像を追加
            foreach (var file in pngFiles)
            {
                if (!group.ImagePaths.Contains(file))
                {
                    group.ImagePaths.Add(file);
                }
            }

            // アイテムへのドロップが外側の Behavior に伝わるのを遮断
            e.Handled = true;
        }
    }
}