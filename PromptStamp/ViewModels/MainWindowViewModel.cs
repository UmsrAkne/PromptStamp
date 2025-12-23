using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using PromptStamp.Models;
using PromptStamp.Utils;

namespace PromptStamp.ViewModels;

// ReSharper disable once ClassNeverInstantiated.Global
public class MainWindowViewModel : BindableBase
{
    private readonly AppVersionInfo appVersionInfo = new ();

    private ObservableCollection<ImagePromptGroup> imagePromptGroups = new ();

    private string commonPrompt = string.Empty;

    public MainWindowViewModel()
    {
        SetDummies();
    }

    public string CommonPrompt { get => commonPrompt; set => SetProperty(ref commonPrompt, value); }

    public ObservableCollection<ImagePromptGroup> ImagePromptGroups
    {
        get => imagePromptGroups;
        set => SetProperty(ref imagePromptGroups, value);
    }

    public string Title => appVersionInfo.Title;

    public DelegateCommand ApplyDiffAllCommand => new (() =>
    {
        foreach (var imagePromptGroup in ImagePromptGroups)
        {
            imagePromptGroup.ApplyDiffPrompt(CommonPrompt);
        }
    });

    [Conditional("DEBUG")]
    private void SetDummies()
    {
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        var testDirectoryPath = Path.Combine(desktopPath, "myFiles", "Tests", "RiderProjects", "PromptStamp", "images");
        var files = Directory.GetFiles(testDirectoryPath)
            .Select(p =>
        {
            var ipg = new ImagePromptGroup
            {
                Header = Path.GetFileNameWithoutExtension(p),
            };

            ipg.ImagePaths.Add(p);
            ipg.DiffPrompts.Add(new DiffPrompt()
            {
                Key = "Test Key1",
                Prompt = "Test Prompt1",
            });

            return ipg;
        });

        imagePromptGroups = new ObservableCollection<ImagePromptGroup>(files);

        CommonPrompt = "Common Text, Common Text, Common Text, Common Text,Common Text, Common Text,";
    }
}