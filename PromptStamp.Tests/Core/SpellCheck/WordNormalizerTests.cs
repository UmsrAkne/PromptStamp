using PromptStamp.Core.SpellCheck;

namespace PromptStamp.Tests.Core.SpellCheck;

public class WordNormalizerTests
{
    private WordNormalizer normalizer = null!;

    [SetUp]
    public void SetUp()
    {
        normalizer = new WordNormalizer();
    }

    [Test]
    public void Normalize_NullOrWhiteSpace_ReturnsEmpty()
    {
        Assert.That(normalizer.Normalize(null!), Is.EqualTo(string.Empty));
        Assert.That(normalizer.Normalize(string.Empty), Is.EqualTo(string.Empty));
        Assert.That(normalizer.Normalize("   \t\n"), Is.EqualTo(string.Empty));
    }

    [Test]
    public void Normalize_TrimsWhitespace()
    {
        Assert.That(normalizer.Normalize("  apple  "), Is.EqualTo("apple"));
    }

    [Test]
    public void Normalize_RemovesSurroundingParentheses_OnceOrNested()
    {
        Assert.That(normalizer.Normalize("(apple)"), Is.EqualTo("apple"));
        Assert.That(normalizer.Normalize("((banana))"), Is.EqualTo("banana"));
        Assert.That(normalizer.Normalize("(((cherry:0.5)))"), Is.EqualTo("cherry"));
    }

    [Test]
    public void Normalize_RemovesNumericWeightSuffix()
    {
        Assert.That(normalizer.Normalize("hair:0.8"), Is.EqualTo("hair"));
        Assert.That(normalizer.Normalize("(short hair:1)"), Is.EqualTo("short hair"));
        Assert.That(normalizer.Normalize("(short hair:0.9)"), Is.EqualTo("short hair"));
    }

    [Test]
    public void Normalize_RemovesNumericPrefix()
    {
        Assert.That(normalizer.Normalize("1girl"), Is.EqualTo("girl"));
        Assert.That(normalizer.Normalize("12girl"), Is.EqualTo("girl"));
    }

    [Test]
    public void Normalize_DoesNotRemoveNonNumericWeightSuffix()
    {
        Assert.That(normalizer.Normalize("style:strong"), Is.EqualTo("style:strong"));
    }

    [Test]
    public void Normalize_TextWithNoAlphabet_ReturnsEmpty()
    {
        Assert.That(normalizer.Normalize("12345"), Is.EqualTo(string.Empty));
        Assert.That(normalizer.Normalize("(123:0.7)"), Is.EqualTo(string.Empty));
    }

    [Test]
    public void Normalize_UsesLastColonForWeight()
    {
        // After removing the numeric weight using the last colon, the remaining part may still contain a colon
        Assert.That(normalizer.Normalize("face:smile:0.5"), Is.EqualTo("face:smile"));
    }
}