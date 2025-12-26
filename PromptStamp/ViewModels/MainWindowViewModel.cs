using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using PromptStamp.Factories;
using PromptStamp.Models;
using PromptStamp.Utils;
using PromptStamp.Utils.Log;

namespace PromptStamp.ViewModels;

// ReSharper disable once ClassNeverInstantiated.Global
public class MainWindowViewModel : BindableBase
{
    private readonly AppVersionInfo appVersionInfo = new ();

    private string commonPrompt = string.Empty;
    private DiffPrompt pendingDiffPrompt = new ();

    public MainWindowViewModel()
    {
        SetDummies();
    }

    public MainWindowViewModel(IAppLogger logger)
    {
        Logger = logger;
        Logger.Info("MainViewModel initialized");
        PromptGroupListViewModel = new PromptGroupListViewModel(Logger);
        SetDummies();
    }

    public string CommonPrompt { get => commonPrompt; set => SetProperty(ref commonPrompt, value); }

    public PromptGroupListViewModel PromptGroupListViewModel { get; }

    public DiffPrompt PendingDiffPrompt
    {
        get => pendingDiffPrompt;
        set => SetProperty(ref pendingDiffPrompt, value);
    }

    public string Title => appVersionInfo.Title;

    public DelegateCommand ApplyDiffAllCommand => new (() =>
    {
        foreach (var imagePromptGroup in PromptGroupListViewModel.Items)
        {
            imagePromptGroup.ApplyDiffPrompt(CommonPrompt);
        }
    });

    public DelegateCommand AddDiffPromptAllCommand => new DelegateCommand(() =>
    {
        var anyAdded = PromptGroupListViewModel.Items
            .Select(imagePromptGroup => imagePromptGroup.TryAddDiffPrompt(PendingDiffPrompt.Clone())).ToList();

        if (anyAdded.Any(b => b))
        {
            PendingDiffPrompt = new DiffPrompt();
        }
    });

    public DelegateCommand<ImagePromptGroup> AddDiffPromptCommand => new ((param) =>
    {
        if (param == null)
        {
            return;
        }

        param.DiffPrompts.Add(new DiffPrompt());
    });

    public IAppLogger Logger { get; }

    [Conditional("DEBUG")]
    private void SetDummies()
    {
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        var testDirectoryPath = Path.Combine(desktopPath, "myFiles", "Tests", "RiderProjects", "PromptStamp", "images");
        var files = Directory.GetFiles(testDirectoryPath)
            .Select(p =>
            {
            var ipg = ImagePromptGroupFactory.Create();
            ipg.Header = Path.GetFileNameWithoutExtension(p);

            ipg.ImagePaths.Add(p);
            ipg.TryAddDiffPrompt(new DiffPrompt()
            {
                Key = "Test Key1",
                Prompt = "Test Prompt1",
            });

            return ipg;
        });

        PromptGroupListViewModel.Items = new ObservableCollection<ImagePromptGroup>(files);

        CommonPrompt = "Common Text, Common Text, Common Text, Common Text,Common Text, Common Text,";
    }
}