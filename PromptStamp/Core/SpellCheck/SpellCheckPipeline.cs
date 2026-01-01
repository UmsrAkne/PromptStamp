using System;
using System.IO;
using PromptStamp.Utils.Log;

namespace PromptStamp.Core.SpellCheck
{
    public class SpellCheckPipeline
    {
        private readonly WordNormalizer normalizer = new ();
        private readonly SpellChecker spellChecker;

        private readonly IAppLogger logger;

        public SpellCheckPipeline(IAppLogger logger)
        {
            this.logger = logger;

            // Resolve dictionary files relative to the application's base directory
            var baseDir = AppContext.BaseDirectory;
            var dictDir = Path.Combine(baseDir, "Dictionaries");
            var affPath = Path.Combine(dictDir, "en_US.aff");
            var dicPath = Path.Combine(dictDir, "en_US.dic");

            spellChecker = new SpellChecker(affPath, dicPath);
        }

        public int LastIssueCount { get; private set; }

        public void Check(string text, string location)
        {
            LastIssueCount = 0;

            foreach (var token in TextTokenizer.Tokenize(text))
            {
                var word = normalizer.Normalize(token);
                if (string.IsNullOrEmpty(word))
                {
                    continue;
                }

                var result = spellChecker.Check(word);

                if (!result.IsCorrect)
                {
                    LastIssueCount++;
                    ReportMissSpell(result, location);
                }
            }
        }

        private void ReportMissSpell(SpellCheckResult result, string location)
        {
            logger.Warn($"[SpellCheck] {location}: {result.Word}");
        }
    }
}