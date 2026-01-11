using PromptStamp.Core;

namespace PromptStamp.Tests.Core
{
    [TestFixture]
    public class EmphasisAdjusterTests
    {
        // ----------------------------
        // 通常トークン → 強調構文
        // ----------------------------

        [Test]
        public void PlainWord_Increase_AddsEmphasis()
        {
            var result = EmphasisAdjuster.Adjust("bbb", +0.1);

            Assert.That(result, Is.EqualTo("(bbb:1.1)"));
        }

        [Test]
        public void PlainWord_Decrease()
        {
            var result = EmphasisAdjuster.Adjust("bbb", -0.1);

            Assert.That(result, Is.EqualTo("(bbb:0.9)"));
        }

        // ----------------------------
        // 強調構文 → 増減
        // ----------------------------

        [Test]
        public void Emphasis_Increase_IncrementsValue()
        {
            var result = EmphasisAdjuster.Adjust("(bbb:1.1)", +0.1);

            Assert.That(result, Is.EqualTo("(bbb:1.2)"));
        }

        [Test]
        public void Emphasis_Decrease_DecrementsValue()
        {
            var result = EmphasisAdjuster.Adjust("(bbb:1.2)", -0.1);

            Assert.That(result, Is.EqualTo("(bbb:1.1)"));
        }

        // ----------------------------
        // 1.0 になったら構文解除
        // ----------------------------

        [Test]
        public void Emphasis_Decrease_ToOne_RemovesSyntax()
        {
            var result = EmphasisAdjuster.Adjust("(bbb:1.1)", -0.1);

            Assert.That(result, Is.EqualTo("bbb"));
        }

        [Test]
        public void Emphasis_Decrease_FromOne_UpdatesValue()
        {
            var result = EmphasisAdjuster.Adjust("(bbb:1.0)", -0.1);

            Assert.That(result, Is.EqualTo("(bbb:0.9)"));
        }

        [Test]
        public void Emphasis_Decrease_DecrementsValue_EvenIfOne()
        {
            var result = EmphasisAdjuster.Adjust("(bbb:0.9)", -0.1);

            Assert.That(result, Is.EqualTo("(bbb:0.8)"));
        }

        // ----------------------------
        // 複合語（スペース含み）
        // ----------------------------

        [Test]
        public void MultiWord_Increase_WrappedAsSingleToken()
        {
            var result = EmphasisAdjuster.Adjust("two word", +0.1);

            Assert.That(result, Is.EqualTo("(two word:1.1)"));
        }

        [Test]
        public void MultiWordEmphasis_Decrease_RemovesSyntax()
        {
            var result = EmphasisAdjuster.Adjust("(two word:1.1)", -0.1);

            Assert.That(result, Is.EqualTo("two word"));
        }

        // ----------------------------
        // トリム・安全系
        // ----------------------------

        [Test]
        public void Token_WithSpaces_IsHandledCorrectly()
        {
            var result = EmphasisAdjuster.Adjust("  bbb  ", +0.1);

            Assert.That(result, Is.EqualTo("(bbb:1.1)"));
        }

        [Test]
        public void EmptyToken_NoChange()
        {
            var result = EmphasisAdjuster.Adjust("", +0.1);

            Assert.That(result, Is.EqualTo(""));
        }
    }
}