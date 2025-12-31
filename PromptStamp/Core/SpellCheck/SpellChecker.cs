using System;
using System.Collections.Generic;
using System.Linq;

namespace PromptStamp.Core.SpellCheck
{
    public class SpellChecker
    {
        private readonly HashSet<string> dictionary;

        public SpellChecker(IEnumerable<string> words)
        {
            dictionary = new HashSet<string>(
                words.Select(w => w.ToLowerInvariant()));
        }

        public SpellCheckResult Check(string word)
        {
            var key = word.ToLowerInvariant();
            var ok = dictionary.Contains(key);

            return new SpellCheckResult(
                word,
                ok,
                Array.Empty<string>());
        }
    }
}