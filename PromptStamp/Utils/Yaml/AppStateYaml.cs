using System.Collections.Generic;

namespace PromptStamp.Utils.Yaml
{
    public class AppStateYaml
    {
        public string CommonPrompt { get; set; }

        public List<ImagePromptGroupYaml> Groups { get; set; }
    }
}