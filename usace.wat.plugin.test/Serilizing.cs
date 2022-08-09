using Xunit;
using YamlDotNet;
using YamlDotNet.Serialization;

namespace usace.wat.plugin.test
{
    public class Serilizing
    {
        [Fact]
        public void SerializeToYAML()
        {
            AWSConfig aws = new AWSConfig();

            var config = new Configuration();
            var serilizer = new Serializer();
            serilizer.Serialize(aws);


        }
    }
}