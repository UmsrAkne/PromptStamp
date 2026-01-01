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

                           1girl, black hair, <lora-name:1.0>, (short hair:0.8),
                           Negative prompt: worst quality, 
                           Steps: 12, Sampler: Euler, Schedule type: Automatic, CFG scale: 7, Seed: 3793887137, Size: 480x672, Model hash: adfsf010ff, Model: checkPointNameXX, VAE hash: b4958602ab, VAE: xlVAEC_c1.safetensors, Version: v1.10.1
                           """;

            var tokens = TextTokenizer.Tokenize(metadata);

            // 強調構文 (xxx yyy:1.0) はカッコや数値部分も含めてそのまま分割される。
            // 不要な部分の削除はこの後の Normalizer が行う。
            // また、スペルチェック不要であるため、Lora の指定は削除される。

            CollectionAssert.AreEqual(
                new List<string>
                {
                    "masterpiece",
                    "BREAK",
                    "1girl",
                    "black",
                    "hair",
                    "(short",
                    "hair:0.8)",
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