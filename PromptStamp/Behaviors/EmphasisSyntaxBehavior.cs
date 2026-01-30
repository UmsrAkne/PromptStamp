using System;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using PromptStamp.Core;

namespace PromptStamp.Behaviors
{
    public class EmphasisSyntaxBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
        }

        private static TextRange? FindTokenRange(string text, int caretIndex)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            caretIndex = Math.Clamp(caretIndex, 0, text.Length);

            var start = caretIndex;
            var end = caretIndex;

            // 左へ
            while (start > 0)
            {
                var c = text[start - 1];
                if (c is ',' or '\n')
                {
                    break;
                }

                start--;
            }

            // 右へ
            while (end < text.Length)
            {
                var c = text[end];
                if (c is ',' or '\n')
                {
                    break;
                }

                end++;
            }

            // トリム（区切り周辺の空白）
            while (start < end && text[start] == ' ')
            {
                start++;
            }

            while (end > start && text[end - 1] == ' ')
            {
                end--;
            }

            if (start >= end)
            {
                return null;
            }

            return new TextRange(start, end - start);
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
            {
                return;
            }

            var delta =
                e.Key switch
                {
                    Key.Up => +0.1,
                    Key.Down => -0.1,
                    _ => 0,
                };

            if (delta == 0)
            {
                return;
            }

            var tb = AssociatedObject;
            var range = FindTokenRange(tb.Text, tb.CaretIndex);
            if (range == null)
            {
                return;
            }

            var token = tb.Text.Substring(range.Value.Start, range.Value.Length);
            var newToken = LoraStrengthAdjuster.CanHandle(token)
                ? LoraStrengthAdjuster.Adjust(token, delta)
                : EmphasisAdjuster.Adjust(token, delta);

            tb.Text = tb.Text[..range.Value.Start] + newToken + tb.Text[(range.Value.Start + range.Value.Length) ..];

            tb.CaretIndex = range.Value.Start + newToken.Length;
            e.Handled = true;
        }

        private readonly struct TextRange
        {
            public TextRange(int start, int length)
            {
                Start = start;
                Length = length;
            }

            public int Start { get; }

            public int Length { get; }
        }
    }
}