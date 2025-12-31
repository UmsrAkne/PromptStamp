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

        public void Check(string text)
        {
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
                    ReportMissSpell(result);
                }
            }
        }

        private void ReportMissSpell(SpellCheckResult result)
        {
            logger.Warn($"[SpellCheck] {result.Word}");
        }
    }
}