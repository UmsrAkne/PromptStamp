using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Xaml.Behaviors;
using PromptStamp.ViewModels;

namespace PromptStamp.Behaviors
{
    public class PngMetadataDropBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            // TextBox の場合、 Drop, DragOver は内部で処理されてしまうため、Preview の方をハンドルする。
            AssociatedObject.AllowDrop = true;
            AssociatedObject.PreviewDrop += OnDrop;
            AssociatedObject.PreviewDragOver += OnDragOver;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewDrop -= OnDrop;
            AssociatedObject.PreviewDragOver -= OnDragOver;
        }

        private static bool IsPng(string path)
        {
            return Path.GetExtension(path).Equals(".png", System.StringComparison.OrdinalIgnoreCase);
        }

        private static string ReadPngTextMetadata(string path)
        {
            using var stream = File.OpenRead(path);
            var decoder =
                new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

            var metadata = decoder.Frames[0].Metadata as BitmapMetadata;
            if (metadata == null)
            {
                return null;
            }

            // 例：Stable Diffusion がよく使う tEXt
            if (metadata.ContainsQuery("/tEXt/parameters"))
            {
                return metadata.GetQuery("/tEXt/parameters") as string;
            }

            // fallback
            foreach (var query in metadata)
            {
                if (metadata.GetQuery(query) is string s && !string.IsNullOrWhiteSpace(s))
                {
                    return s;
                }
            }

            return null;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files!.Any(IsPng))
                {
                    e.Effects = DragDropEffects.Copy;
                    e.Handled = true;
                    return;
                }
            }

            e.Effects = DragDropEffects.None;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var png = files!.FirstOrDefault(IsPng);
            if (png == null)
            {
                return;
            }

            var text = ReadPngTextMetadata(png);
            if (text == null)
            {
                return;
            }

            if (AssociatedObject.DataContext is MainWindowViewModel vm)
            {
                vm.CommonPrompt = text;
            }
        }
    }
}