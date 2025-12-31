using PromptStamp.Utils.Log;

namespace PromptStamp.Core.SpellCheck
{
    public class SpellCheckPipeline
    {
        private readonly WordNormalizer normalizer = new ();
        private readonly SpellChecker spellChecker = new (
            "Dictionaries/en_US.aff",
            "Dictionaries/en_US.dic");

        private readonly IAppLogger logger;

        public SpellCheckPipeline(IAppLogger logger)
        {
            this.logger = logger;
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