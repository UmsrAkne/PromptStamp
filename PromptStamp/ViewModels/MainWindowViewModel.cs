using System.Collections.ObjectModel;
using Prism.Mvvm;
using PromptStamp.Models;
using PromptStamp.Utils;

namespace PromptStamp.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private readonly AppVersionInfo appVersionInfo = new ();

    private ObservableCollection<ImagePromptGroup> imagePromptGroups = new ();

    private string commonPrompt = string.Empty;

    public string CommonPrompt { get => commonPrompt; set => SetProperty(ref commonPrompt, value); }

    public ObservableCollection<ImagePromptGroup> ImagePromptGroups
    {
        get => imagePromptGroups;
        set => SetProperty(ref imagePromptGroups, value);
    }

    public string Title => appVersionInfo.Title;
}

// ReSharper disable once ClassNeverInstantiated.Global