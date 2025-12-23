using PromptStamp.Models;
using PromptStamp.Utils.Log;

namespace PromptStamp.Factories
{
    public static class ImagePromptGroupFactory
    {
        public static IAppLogger Logger { get; set; }

        public static ImagePromptGroup Create()
        {
            var imagePromptGroup = new ImagePromptGroup(Logger);
            return imagePromptGroup;
        }
    }
}