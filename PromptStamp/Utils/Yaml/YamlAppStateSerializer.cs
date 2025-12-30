using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace PromptStamp.Utils.Yaml
{
    public class YamlAppStateSerializer
    {
        private readonly ISerializer serializer = new SerializerBuilder()
            .WithEventEmitter(inner => new LiteralMultilineEmitter(inner))
            .Build();

        private readonly IDeserializer deserializer = new DeserializerBuilder().Build();

        public string Serialize(AppStateYaml dto)
        {
            return serializer.Serialize(dto);
        }

        public AppStateYaml Deserialize(string yamlString)
        {
            return deserializer.Deserialize<AppStateYaml>(yamlString);
        }

        private class LiteralMultilineEmitter : ChainedEventEmitter
        {
            public LiteralMultilineEmitter(IEventEmitter nextEmitter)
                : base(nextEmitter)
            {
            }

            public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
            {
                if (eventInfo.Source.Value is string s && s.Contains('\n'))
                {
                    eventInfo = new ScalarEventInfo(eventInfo.Source)
                    {
                        Style = ScalarStyle.Literal,
                    };
                }

                base.Emit(eventInfo, emitter);
            }
        }
    }
}