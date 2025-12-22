using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png.Chunks;

namespace PromptStamp.Utils
{
    public class MetadataWriter
    {
        public static void Write(string imagePath, string metadataText)
        {
            using var image = Image.Load(imagePath);
            var pngMeta = image.Metadata.GetPngMetadata();

            // 既存 テキストを全削除
            pngMeta.TextData.Clear();

            // 新しい parameters を追加
            pngMeta.TextData.Add(new PngTextData(
                "parameters",
                metadataText,
                null,
                null));

            image.Save(imagePath);
        }
    }
}