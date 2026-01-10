using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Prism.Ioc;
using PromptStamp.Behaviors;
using PromptStamp.Factories;
using PromptStamp.Utils;
using PromptStamp.Utils.Log;
using PromptStamp.Views;

namespace PromptStamp;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IAppLogger, InMemoryAppLogger>();

        var logger = Container.Resolve<IAppLogger>();

        ImagePromptGroupFactory.Logger = logger;
        MetadataWriter.Logger = logger;
        ImageListCutPasteBehavior.Logger = logger;

        logger.Info("MetadataWriter and ImagePromptGroupFactory logger initialized");
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 1. UIスレッドの未処理例外
        DispatcherUnhandledException += (_, args) =>
        {
            LogAndShowFatal(args.Exception, "DispatcherUnhandledException");
            args.Handled = true;
        };

        // 2. UI外のスレッドでの未処理例外
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                LogAndShowFatal(ex, "AppDomain.UnhandledException");
            }
        };

        // 3. Task で拾われなかった例外
        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            LogAndShowFatal(args.Exception, "TaskScheduler.UnobservedTaskException");
            args.SetObserved();
        };
    }

    private void LogAndShowFatal(Exception ex, string source)
    {
        try
        {
            var logger = Container.Resolve<IAppLogger>();
            logger.Error($"[{source}] {ex}");
        }
        catch
        {
            // logger 自体が死んでる場合の最終ルート
            var baseDir = AppContext.BaseDirectory;
            var path = Path.Combine(baseDir, "error.txt");

            File.AppendAllText(
                path,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{source}] {ex}\n\n");
        }

        MessageBox.Show(
            $"Fatal Error ({source}):\n{ex.Message}",
            "App Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}