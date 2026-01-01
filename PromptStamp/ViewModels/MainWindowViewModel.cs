using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Prism.Commands;
using Prism.Mvvm;
using PromptStamp.Core.SpellCheck;
using PromptStamp.Factories;
using PromptStamp.Models;
using PromptStamp.Utils;
using PromptStamp.Utils.Log;
using PromptStamp.Utils.Yaml;
using YamlDotNet.Serialization;

namespace PromptStamp.ViewModels;

// ReSharper disable once ClassNeverInstantiated.Global
public class MainWindowViewModel : BindableBase
{
    private readonly AppVersionInfo appVersionInfo = new ();
    private readonly SpellCheckPipeline spellCheckPipeline;

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
        spellCheckPipeline = new SpellCheckPipeline(Logger);
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

    public DelegateCommand<ImagePromptGroup> CopyResolvedPromptCommand => new ((group) =>
    {
        if (group == null)
        {
            Logger.Warn("CopyResolvedPromptCommand: group parameter was null. Operation aborted.");
            return;
        }

        Clipboard.SetText(group.ApplyReplacement(CommonPrompt));
        Logger.Info("Resolved prompt copied to clipboard.");
    });

    public AsyncRelayCommand OpenAppStateWithYamlCommand => new (async () =>
    {
        try
        {
            // 1. local_data ディレクトリ作成
            var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            var localDataDir = Path.Combine(exeDir, "local_data");
            Directory.CreateDirectory(localDataDir);

            // 2. VM → DTO 変換
            var dto = AppStateMapper.ToDto(this);

            // 3. YAML 作成
            var serializer = new YamlAppStateSerializer();
            var yaml = serializer.Serialize(dto);

            // 4. 保存ファイルパス（ユニーク名）
            var filePath = Path.Combine(localDataDir, $"app_state_{DateTime.Now:yyyyMMdd_HHmmss}.yaml");

            await File.WriteAllTextAsync(filePath, yaml, Encoding.UTF8);

            // 5. OS の既定エディタで開く
            var psi = new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true,
            };

            var process = Process.Start(psi);

            if (process != null)
            {
                // 6. エディタが閉じるまで待つ
                await Task.Run(() => process.WaitForExit());
            }
            else
            {
                Logger.Warn("外部エディタを起動できませんでした。");
                return;
            }

            // 7. YAML 再読み込み（例外をキャッチしたら反映しない）
            AppStateYaml reloadedDto;
            try
            {
                var text = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
                var deserializer = new DeserializerBuilder().Build();
                reloadedDto = deserializer.Deserialize<AppStateYaml>(text);
            }
            catch (Exception ex)
            {
                Logger?.Error("YAML の読み込みで例外が発生したため、反映を中止します。", ex);
                return;
            }

            // 8. DTO → ViewModel 適用
            try
            {
                AppStateMapper.ApplyToViewModel(reloadedDto, this);
            }
            catch (Exception ex)
            {
                Logger?.Error("AppState の適用で例外が発生しました。反映は行われません。", ex);
            }
        }
        catch (Exception ex)
        {
            Logger?.Error("AppStateWithYamlCommand の処理中に例外が発生しました。", ex);
        }
    });

    public IAppLogger Logger { get; }

    public DelegateCommand SpellCheckCommand => new DelegateCommand(() =>
    {
        List<(string parentGroupName, string prompt)> prompts = new ()
        {
            ("Common Prompt", CommonPrompt),
        };

        foreach (var group in PromptGroupListViewModel.Items)
        {
            foreach (var groupDiffPrompt in group.DiffPrompts)
            {
                 prompts.Add((group.Header, groupDiffPrompt.Prompt));
            }
        }

        var issueCount = 0;
        foreach (var valueTuple in prompts)
        {
            spellCheckPipeline.Check(valueTuple.prompt, valueTuple.parentGroupName);
            issueCount += spellCheckPipeline.LastIssueCount;
        }

        Logger.Info(issueCount == 0
            ? "SpellCheck: no spelling issues detected."
            : $"SpellCheck: {issueCount} issues found.");
    });

    public DelegateCommand<string> WriteMetadataToImageCommand => new ((param) =>
    {
        if (PromptGroupListViewModel.SelectedItem == null)
        {
            return;
        }

        PromptGroupListViewModel.SelectedItem.ApplyDiffPrompt(CommonPrompt, param);
    });

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