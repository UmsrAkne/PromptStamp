using System.Text.RegularExpressions;

namespace PromptStamp.Core.SpellCheck
{
    public class WordNormalizer
    {
        public string Normalize(string token)
        {
            if (string.IsNullOrWhiteSpace(token) || IsAngleBracketToken(token))
            {
                return string.Empty;
            }

            // カッコを除去
            var t = RemoveParentheses(token);

            // :数字 の強調値を除去 (例: hair:0.8 → hair)
            t = RemoveWeight(t);

            t = t.Trim();

            // 先頭に数字がつく場合は削除
            t = RemovePrefixNumerics(t);

            // アルファベットが含まれない場合は無視
            if (!ContainsAlphabet(t))
            {
                return string.Empty;
            }

            return t;
        }

        private static bool IsAngleBracketToken(string s)
        {
            // <xxx> の形式のみ対象
            return s.Length >= 2 && s.StartsWith("<") && s.EndsWith(">");
        }

        private static string RemovePrefixNumerics(string s)
        {
            var regex = new Regex("^[0-9]+");
            while (regex.IsMatch(s) && s.Length > 0)
            {
                s = s.Substring(1);
            }

            return s;
        }

        private static string RemoveParentheses(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            // 例: "(short hair:0.8)" → "short hair:0.8"
            // カッコをお含む場合は、必然的に文字数が２文字以上になるため、それを条件に含める。
            while (s.Length >= 2 && s.StartsWith("(") && s.EndsWith(")"))
            {
                s = s.Substring(1, s.Length - 2);
            }

            return s;
        }

        private static string RemoveWeight(string s)
        {
            // 形式: xxx:数字
            // 例: hair:0.8 → hair
            var idx = s.LastIndexOf(':');
            if (idx > 0)
            {
                var weightPart = s.Substring(idx + 1).Trim();
                if (double.TryParse(weightPart, out _))
                {
                    return s.Substring(0, idx);
                }
            }

            return s;
        }

        private static bool ContainsAlphabet(string s)
        {
            foreach (var c in s)
            {
                if (c is >= 'a' and <= 'z' or >= 'A' and <= 'Z')
                {
                    return true;
                }
            }

            return false;
        }
    }
}