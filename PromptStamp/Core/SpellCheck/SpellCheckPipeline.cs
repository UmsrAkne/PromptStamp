using System.Collections.Generic;

namespace PromptStamp.Core.SpellCheck
{
    public class SpellCheckPipeline
    {
        private readonly WordNormalizer normalizer = new WordNormalizer();
        private readonly SpellChecker spellChecker = new SpellChecker(new List<string>());

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
                    // ログ / UI / マーカー表示
                }
            }
        }
    }
}