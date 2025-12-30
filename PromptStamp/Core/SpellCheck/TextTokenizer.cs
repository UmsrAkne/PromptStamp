using System;
using System.Collections.Generic;

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

        /// <summary>
        /// 入力されたテキストを Separators に設定されている文字で分割します。
        /// </summary>
        /// <param name="text">分割したい文字列</param>
        /// <returns>分割された単語のリスト</returns>
        public static IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                yield break;
            }

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
    }
}