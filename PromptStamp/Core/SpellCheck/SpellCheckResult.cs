using System.Collections.Generic;

namespace PromptStamp.Core.SpellCheck
{
    public record SpellCheckResult(
        string Word,
        bool IsCorrect,
        IReadOnlyList<string> Suggestions);
}