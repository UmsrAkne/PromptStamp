using Prism.Mvvm;
using PromptStamp.Utils;

namespace PromptStamp.ViewModels;

// ReSharper disable once ClassNeverInstantiated.Global
public class MainWindowViewModel : BindableBase
{
    private readonly AppVersionInfo appVersionInfo = new ();

    public string Title => appVersionInfo.Title;
}