using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PromptStamp.Core
{
    public static class EmphasisAdjuster
    {
        private const double Step = 0.1;
        private const double MinValue = 0.1;
        private const double Epsilon = Step / 100;

        // (two word:1.1)
        private readonly static Regex EmphasisRegex =
            new (@"^\((.+?):([0-9]+(?:\.[0-9]+)?)\)$", RegexOptions.Compiled);

        public static string Adjust(string token, double delta)
        {
            token = token.Trim();

            if (string.IsNullOrEmpty(token))
            {
                return token;
            }

            if (TryParseEmphasis(token, out var word, out var value))
            {
                var newValue = Math.Round(value + delta, 1);

                if (Math.Abs(newValue - 1.0) < Epsilon)
                {
                    // 1.0 になったら構文解除
                    return word;
                }

                newValue = Math.Max(newValue, MinValue);
                return Format(word, newValue);
            }
            else
            {
                // 通常単語 → 強調構文に変換
                var newValue = Math.Round(1.0 + delta, 1);

                if (Math.Abs(newValue - 1.0) < Epsilon)
                {
                    return token;
                }

                return Format(token, newValue);
            }
        }

        private static bool TryParseEmphasis(string token, out string word, out double value)
        {
            var match = EmphasisRegex.Match(token);
            if (!match.Success)
            {
                word = string.Empty;
                value = 0;
                return false;
            }

            word = match.Groups[1].Value;

            return double.TryParse(
                match.Groups[2].Value,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out value);
        }

        private static string Format(string word, double value)
        {
            return $"({word}:{value.ToString("0.0", CultureInfo.InvariantCulture)})";
        }
    }
}