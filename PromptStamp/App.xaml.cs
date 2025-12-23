using System.Windows;
using Prism.Ioc;
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

        logger.Info("MetadataWriter and ImagePromptGroupFactory logger initialized");
    }
}