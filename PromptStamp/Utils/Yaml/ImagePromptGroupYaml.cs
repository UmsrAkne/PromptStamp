using System.Collections.Generic;

namespace PromptStamp.Utils.Yaml
{
    public class ImagePromptGroupYaml
    {
        public string Header { get; set; }

        public List<string> ImagePaths { get; set; }

        public List<DiffPromptYaml> DiffPrompts { get; set; }
    }
}