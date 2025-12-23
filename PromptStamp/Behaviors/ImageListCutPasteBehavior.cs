using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using PromptStamp.Models;
using PromptStamp.Utils.Log;
using PromptStamp.ViewModels;

namespace PromptStamp.Behaviors
{
    public class ImageListCutPasteBehavior : Behavior<ListBox>
    {
        public static IAppLogger Logger { private get; set; }

        private CutImageBuffer Buffer { get; } = new ();

        protected override void OnAttached()
        {
            Logger.Info("ImageListCutPasteBehavior OnAttached()");
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            Logger.Info("ImageListCutPasteBehavior OnDetaching()");
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.X)
            {
                Cut();
                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.V)
            {
                Paste();
                e.Handled = true;
            }
        }

        private void Cut()
        {
            Logger.Info("ImageListCutPasteBehavior Cut()");
            if (AssociatedObject.SelectedItem is not string path)
            {
                return;
            }

            var vm = AssociatedObject.DataContext as MainWindowViewModel;

            if (vm == null)
            {
                return;
            }

            var target = vm.PromptGroupListViewModel.SelectedItem;
            Buffer.ImagePath = path;
            Buffer.SourceGroup = target;

            Logger.Info($"Buffer.ImagePath: {Buffer.ImagePath}");
        }

        private void Paste()
        {
            Logger.Info("ImageListCutPasteBehavior Paste()");
            if (!Buffer.HasItem)
            {
                Logger.Info("Buffer item is null.");
                return;
            }

            if (AssociatedObject.DataContext is not MainWindowViewModel vm)
            {
                return;
            }

            var targetGroup = vm.PromptGroupListViewModel.SelectedItem;

            if (Buffer.SourceGroup == targetGroup)
            {
                Logger.Info("targetGroup == Buffer.SourceGroup");
                return; // 同じ場所は何もしない
            }

            Buffer.SourceGroup?.ImagePaths.Remove(Buffer.ImagePath!);
            targetGroup.ImagePaths.Add(Buffer.ImagePath!);

            Logger.Info($"Pasted {Buffer.ImagePath}");

            Buffer.ImagePath = null;
            Buffer.SourceGroup = null;
        }
    }
}