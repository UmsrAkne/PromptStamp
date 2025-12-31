using PromptStamp.Core.SpellCheck;

namespace PromptStamp.Tests.Core.SpellCheck
{
    [TestFixture]
    public class TextTokenizerTests
    {
        [Test]
        public void Tokenize_Test()
        {
            var metadata = """
                           masterpiece, 
                           BREAK,

                           1girl, black hair, 
                           Negative prompt: worst quality, 
                           Steps: 12, Sampler: Euler, Schedule type: Automatic, CFG scale: 7, Seed: 3793887137, Size: 480x672, Model hash: adfsf010ff, Model: checkPointNameXX, VAE hash: b4958602ab, VAE: xlVAEC_c1.safetensors, Version: v1.10.1
                           """;

            var tokens = TextTokenizer.Tokenize(metadata);

            CollectionAssert.AreEqual(
                new List<string>
                {
                    "masterpiece",
                    "BREAK",
                    "1girl",
                    "black",
                    "hair",
                    "Negative",
                    "prompt:",
                    "worst",
                    "quality",
                },
                tokens
                );
        }
    }
}