using YamlDotNet.Serialization;

namespace PromptStamp.Utils.Yaml
{
    public class YamlAppStateSerializer
    {
        private readonly ISerializer serializer = new SerializerBuilder().Build();
        private readonly IDeserializer deserializer = new DeserializerBuilder().Build();

        public string Serialize(AppStateYaml dto)
        {
            return serializer.Serialize(dto);
        }

        public AppStateYaml Deserialize(string yamlString)
        {
            return deserializer.Deserialize<AppStateYaml>(yamlString);
        }
    }
}