namespace Usace.CC.Plugin
{
  public class AWSConfig
  {
    public string aws_config_name { get; set; }
    public string aws_access_key_id { get; set; }
    public string aws_secret_access_key_id { get; set; }
    public string aws_region { get; set; }
    public string aws_bucket { get; set; }
    /// <summary>
    /// aws_mock is set to True when using minio
    /// </summary>
    public bool aws_mock { get; set; }
    /// <summary>
    /// aws_endpoint is used with minio
    /// for example: http://192.168.206.129:9000
    /// </summary>
    public string aws_endpoint { get; set; }
    private bool aws_disable_ssl { get; set; }
    private bool aws_force_path_style { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="profileName">prefix to environment variable names for connection to S3/minio</param>
    public AWSConfig(string profileName)
    {
      aws_region = Utility.GetEnv(profileName + "_" + EnvironmentVariables.AWS_DEFAULT_REGION);
      aws_access_key_id = Utility.GetEnv(profileName + "_" + EnvironmentVariables.AWS_ACCESS_KEY_ID);
      aws_secret_access_key_id = Utility.GetEnv(profileName + "_" + EnvironmentVariables.AWS_SECRET_ACCESS_KEY);
      aws_bucket = Utility.GetEnv(profileName + "_" + EnvironmentVariables.AWS_S3_BUCKET);
      aws_mock = Boolean.Parse(Utility.GetEnv(profileName + "_" + "S3_MOCK"));
      aws_endpoint = Utility.GetEnv(profileName + "_" + "S3_ENDPOINT"); // used when S3_MOCK is true
      //aws_disable_ssl = Boolean.Parse(Utility.GetEnv(envPrefix + "_" + "S3_DISABLE_SSL")); // can be used when S3_MOCK is true
      //aws_force_path_style = Boolean.Parse(Utility.GetEnv(envPrefix + "_" + "S3_FORCE_PATH_STYLE"));// can be used when S3_MOCK is true
    }
  }
}