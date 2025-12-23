using System.Windows;
using Prism.Ioc;
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
    }
}