using System.Collections.ObjectModel;
using System.Linq;
using Prism.Mvvm;
using PromptStamp.Utils;
using PromptStamp.Utils.Log;

namespace PromptStamp.Models
{
    public class ImagePromptGroup : BindableBase
    {
        private string header;

        public ImagePromptGroup(IAppLogger appLogger)
        {
            Logger = appLogger;
        }

        public string Header { get => header; set => SetProperty(ref header, value); }

        public ObservableCollection<string> ImagePaths { get; set; } = new ();

        public ObservableCollection<DiffPrompt> DiffPrompts { get; set; } = new ();

        private IAppLogger Logger { get; }

        public void ApplyDiffPrompt(string basePrompt, string imagePath = null)
        {
            Logger.Info(string.IsNullOrWhiteSpace(imagePath)
                ? $"ApplyDiffPrompt start: Images={ImagePaths.Count}, Diffs={DiffPrompts.Count}"
                : $"ApplyDiffPrompt start: Image={imagePath}, Diffs={DiffPrompts.Count}");

            var prompt = basePrompt;

            prompt = ApplyReplacement(prompt);

            var targetPaths = imagePath == null
                ? ImagePaths
                : new ObservableCollection<string> { imagePath, };

            foreach (var path in targetPaths)
            {
                MetadataWriter.Write(path, prompt);
            }

            Logger.Info("ApplyDiffPrompt completed");
        }

        /// <summary>
        /// Adds a DiffPrompt to this ImagePromptGroup if possible.
        /// </summary>
        /// <param name="prompt">The DiffPrompt to add.</param>
        /// <returns>
        /// <c>true</c> if the prompt was successfully added to the group;
        /// <c>false</c> if the prompt was not added because the key is empty
        /// or a DiffPrompt with the same normalized key already exists in this group.
        /// </returns>
        /// <remarks>
        /// This method does not throw exceptions for validation failures.
        /// Failing to add a prompt (e.g., due to duplicate keys) is considered a valid outcome
        /// and will be handled by logging and UI notifications.
        /// </remarks>
        public bool TryAddDiffPrompt(DiffPrompt prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt.Key))
            {
                Logger.Warn($"ImagePromptGroup({Header}): prompt.Key is empty.");
                return false;
            }

            var normalizeKeys = DiffPrompts.Select(dp => dp.Key.Trim()).Distinct();
            if (normalizeKeys.Contains(prompt.Key.Trim()))
            {
                Logger.Warn($"ImagePromptGroup({Header}): Duplicate key: {prompt.Key}");
                return false;
            }

            DiffPrompts.Add(prompt);
            return true;
        }

        /// <summary>
        /// DiffPrompts に定義されたキーとプロンプトで、与えられたベースプロンプトの文字列置換を行います。
        /// </summary>
        /// <param name="basePrompt">置換の基準となる元のプロンプト文字列。</param>
        /// <returns>全ての有効な DiffPrompt の置換を適用した後のプロンプト文字列。</returns>
        /// <remarks>
        /// キーは大文字・小文字を区別して一致した部分を単純に Replace します。空白のみのキーは無視されます。
        /// </remarks>
        public string ApplyReplacement(string basePrompt)
        {
            foreach (var diffPrompt in DiffPrompts.Where(dp => !string.IsNullOrWhiteSpace(dp.Key.Trim()) && dp.IsEnabled))
            {
                Logger.Info($"Replace '{diffPrompt.Key}' -> '{diffPrompt.Prompt}'");
                basePrompt = basePrompt.Replace(diffPrompt.Key, diffPrompt.Prompt);
            }

            return basePrompt;
        }
    }
}