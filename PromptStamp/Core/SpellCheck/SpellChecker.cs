using System;
using NHunspell;

namespace PromptStamp.Core.SpellCheck
{
    public class SpellChecker : IDisposable
    {
        private readonly Hunspell hunspell;

        public SpellChecker(string affPath, string dicPath)
        {
            hunspell = new Hunspell(affPath, dicPath);
        }

        public SpellCheckResult Check(string word)
        {
            var isCorrect = hunspell.Spell(word);

            var suggestions = isCorrect
                ? Array.Empty<string>()
                : hunspell.Suggest(word).ToArray();

            return new SpellCheckResult(
                word,
                isCorrect,
                suggestions);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            hunspell.Dispose();
        }
    }
}