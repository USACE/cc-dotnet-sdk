namespace usace.wat.plugin
{
    public class Config
    {
        public AWSConfig[] aws_configs { get; set; }
        public AWSConfig PrimaryConfig() 
        {
            return aws_configs[0];
        }
    }
}
