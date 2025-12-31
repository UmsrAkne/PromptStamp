using System;
using System.Collections.Generic;
using System.Linq;

namespace PromptStamp.Core.SpellCheck
{
    public static class TextTokenizer
    {
        private readonly static char[] Separators =
        {
            ',',  // カンマ
            '\n', // 改行（LF）
            '\r', // 改行（CR）
            '\t', // タブ
            ' ',  // スペース
            '|',  // パイプ
            '_',  // アンダーバー
            '-',  // ハイフン
        };

        private readonly static string[] MetaPrefixes =
        {
            "Steps:",
            "Sampler:",
            "Schedule type:",
            "CFG scale:",
            "Seed:",
            "Size:",
            "Model hash:",
            "Model:",
            "VAE hash:",
            "VAE:",
            "Version:",
        };

        /// <summary>
        /// 入力テキストからスペルチェック対象となる領域だけを抽出し、
        /// Separators に指定された区切り文字で分割します。
        /// メタデータ（Steps 以降の PNG info など）は除外されます。
        /// </summary>
        /// <param name="text">分割対象の文字列</param>
        /// <returns>分割されたトークンの列挙</returns>
        public static IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                yield break;
            }

            // メタデータ（スペルチェック不要な部分）を切り落とす
            text = RemoveMetadataBlock(text);

            // 分割
            var tokens = text.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (var token in tokens)
            {
                var trimmed = token.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed))
                {
                    yield return trimmed;
                }
            }
        }

        private static string RemoveMetadataBlock(string text)
        {
            var lines = text.Split('\n');

            var resultLines = new List<string>();

            foreach (var line in lines)
            {
                // メタデータの行に来たらそれ以降は無視
                if (MetaPrefixes.Any(prefix => line.TrimStart().StartsWith(prefix)))
                {
                    break;
                }

                resultLines.Add(line);
            }

            return string.Join("\n", resultLines);
        }
    }
}