using System;
using System.IO;
using PromptStamp.Utils.Log;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png.Chunks;

namespace PromptStamp.Utils
{
    public class MetadataWriter
    {
        public static IAppLogger Logger { private get; set; }

        public static void Write(string imagePath, string metadataText)
        {
            try
            {
                Logger.Info($"Write metadata: {Path.GetFileName(imagePath)}");

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
            catch (Exception ex)
            {
                Logger.Error($"Metadata write failed: {imagePath}", ex);
                throw;
            }
        }
    }
}