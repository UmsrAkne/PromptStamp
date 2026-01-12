using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PromptStamp.Core
{
    public static class LoraStrengthAdjuster
    {
        private const double MinValue = 0.0;

        // <lora:name:1.0>
        private readonly static Regex LoraRegex = new(@"^<lora:(.+?):([0-9]+(?:\.[0-9]+)?)>$", RegexOptions.Compiled);

        public static string Adjust(string token, double delta)
        {
            token = token.Trim();

            if (string.IsNullOrEmpty(token))
            {
                return token;
            }

            if (!TryParse(token, out var name, out var value))
            {
                // LoRA 構文でなければ何もしない
                return token;
            }

            var newValue = Math.Round(value + delta, 1);
            newValue = Math.Max(newValue, MinValue);

            return Format(name, newValue);
        }

        public static bool CanHandle(string token)
        {
            return !string.IsNullOrWhiteSpace(token) && LoraRegex.IsMatch(token.Trim());
        }

        private static bool TryParse(string token, out string name, out double value)
        {
            var match = LoraRegex.Match(token);
            if (!match.Success)
            {
                name = string.Empty;
                value = 0;
                return false;
            }

            name = match.Groups[1].Value;

            return double.TryParse(
                match.Groups[2].Value,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out value);
        }

        private static string Format(string name, double value)
        {
            return $"<lora:{name}:{value.ToString("0.0", CultureInfo.InvariantCulture)}>";
        }
    }
}