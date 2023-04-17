namespace usace.cc.plugin
{
    public class AWSConfig
    {
        public string aws_config_name { get; set; }
        public string aws_access_key_id { get; set; }
        public string aws_secret_access_key_id { get; set; }
        public string aws_region { get; set; }
        public string aws_bucket { get; set; }
        public bool aws_mock { get; set; }
        public string aws_endpoint { get; set; }
        public bool aws_disable_ssl { get; set; }
        public bool aws_force_path_style { get; set; }
    }
}