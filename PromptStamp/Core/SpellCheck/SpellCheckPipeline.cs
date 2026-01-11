using System;
using System.IO;
using PromptStamp.Utils.Log;

namespace PromptStamp.Core.SpellCheck
{
    public class SpellCheckPipeline
    {
        private readonly WordNormalizer normalizer = new ();
        private readonly SpellChecker spellChecker;
        private readonly bool isSpellCheckEnabled;

        private readonly IAppLogger logger;

        public SpellCheckPipeline(IAppLogger logger)
        {
            this.logger = logger;

            var (affPath, dicPath) = EnsureDictionaryFiles();

            if (IsDictionaryEmpty(affPath, dicPath))
            {
                logger.Warn("[SpellCheck] Dictionary files are empty. Spell check is disabled.");
                isSpellCheckEnabled = false;
                return;
            }

            spellChecker = new SpellChecker(affPath, dicPath);
            isSpellCheckEnabled = true;
            return;

            bool IsDictionaryEmpty(string aPath, string dPath)
            {
                static bool IsEmpty(string path)
                    => !File.Exists(path) || new FileInfo(path).Length == 0;

                return IsEmpty(aPath) || IsEmpty(dPath);
            }
        }

        public int LastIssueCount { get; private set; }

        public void Check(string text, string location)
        {
            LastIssueCount = 0;

            if (!isSpellCheckEnabled)
            {
                return;
            }

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

        private (string affPath, string dicPath) EnsureDictionaryFiles()
        {
            var baseDir = AppContext.BaseDirectory;
            var dictDir = Path.Combine(baseDir, "Dictionaries");

            var affPath = Path.Combine(dictDir, "en_US.aff");
            var dicPath = Path.Combine(dictDir, "en_US.dic");

            try
            {
                // Dictionaries ディレクトリを確実に作成
                Directory.CreateDirectory(dictDir);

                EnsureEmptyFileExists(affPath);
                EnsureEmptyFileExists(dicPath);
            }
            catch (Exception ex)
            {
                logger.Error("Failed to ensure spell dictionary files.", ex);
                throw;
            }

            return (affPath, dicPath);

            void EnsureEmptyFileExists(string path)
            {
                if (File.Exists(path) || string.IsNullOrEmpty(path))
                {
                    return;
                }

                // 空ファイルを作成して即クローズ
                using (File.Create(path))
                {
                }

                logger.Warn($"Spell dictionary file was missing. Created an empty file: {path}");
            }
        }
    }
}