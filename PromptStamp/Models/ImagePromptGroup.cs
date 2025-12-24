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

        public void ApplyDiffPrompt(string basePrompt)
        {
            Logger.Info($"ApplyDiffPrompt start: Images={ImagePaths.Count}, Diffs={DiffPrompts.Count}");

            var prompt = basePrompt;

            foreach (var diffPrompt in DiffPrompts.Where(dp => !string.IsNullOrWhiteSpace(dp.Key.Trim())))
            {
                Logger.Info($"Replace '{diffPrompt.Key}' -> '{diffPrompt.Prompt}'");
                prompt = prompt.Replace(diffPrompt.Key, diffPrompt.Prompt);
            }

            foreach (var path in ImagePaths)
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
    }
}