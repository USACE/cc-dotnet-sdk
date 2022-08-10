namespace usace.wat.plugin
{
    public class AWSConfig
    {
        public string Aws_config_name { get; set; }
        public bool Is_primary_config { get; set; }
        public string Aws_access_key_id { get; set; }
        public string Aws_secret_access_key_id { get; set; }
        public string  Aws_region { get; set; }
        public string Aws_bucket { get; set; }
        public bool Aws_mock { get; set; }
        public string Aws_endpoint { get; set; }
        public string Aws_disable_ssl { get; set; }
        public bool Aws_force_path_style { get; set; }
    }
}