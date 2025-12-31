using System;
using System.Linq;
using WeCantSpell.Hunspell;

namespace PromptStamp.Core.SpellCheck
{
    public class SpellChecker
    {
        private readonly WordList wordList;

        public SpellChecker(string affPath, string dicPath)
        {
            // 静的メソッド CreateFromFiles で読み込み。
            wordList = WordList.CreateFromFiles(dicPath, affPath);
        }

        public SpellCheckResult Check(string word)
        {
            var isCorrect = wordList.Check(word);

            var suggestions = isCorrect
                ? Array.Empty<string>()
                : wordList.Suggest(word).ToArray();

            return new SpellCheckResult(
                word,
                isCorrect,
                suggestions);
        }
    }
}