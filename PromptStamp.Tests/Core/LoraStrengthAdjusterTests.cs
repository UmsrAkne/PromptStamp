using PromptStamp.Core;

namespace PromptStamp.Tests.Core
{
    using NUnit.Framework;

    namespace PromptStamp.Core.Tests
    {
        public class LoraStrengthAdjusterTests
        {
            [TestCase("<lora:test:1.0>", 0.1, "<lora:test:1.1>")]
            [TestCase("<lora:test:1.0>", -0.1, "<lora:test:0.9>")]
            [TestCase("<lora:test:0.5>", 0.2, "<lora:test:0.7>")]
            public void Adjust_ValidLora_IncreaseDecreaseValue( string input, double delta, string expected)
            {
                var result = LoraStrengthAdjuster.Adjust(input, delta);

                Assert.That(result, Is.EqualTo(expected));
            }

            [Test]
            public void Adjust_WhenValueBecomesExactlyOne_ShouldKeepSyntax()
            {
                var result = LoraStrengthAdjuster.Adjust("<lora:test:0.9>", 0.1);

                Assert.That(result, Is.EqualTo("<lora:test:1.0>"));
            }

            [Test]
            public void Adjust_ShouldNotGoBelowMinimum()
            {
                var result = LoraStrengthAdjuster.Adjust("<lora:test:0.0>", -0.5);

                Assert.That(result, Is.EqualTo("<lora:test:0.0>"));
            }

            [TestCase("test")]
            [TestCase("(test:1.1)")]
            [TestCase("<lora:test>")]
            [TestCase("<lora:test:>")]
            public void Adjust_NonLoraToken_ShouldReturnInputAsIs(string input)
            {
                var result = LoraStrengthAdjuster.Adjust(input, 0.1);

                Assert.That(result, Is.EqualTo(input));
            }

            [Test]
            public void Adjust_ShouldTrimWhitespace()
            {
                var result = LoraStrengthAdjuster.Adjust("  <lora:test:1.0>  ", 0.1);

                Assert.That(result, Is.EqualTo("<lora:test:1.1>"));
            }
        }
    }
}