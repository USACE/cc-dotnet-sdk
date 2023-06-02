namespace Usace.CC.Plugin
{
  public static class EnvironmentVariables
  {
    /// <summary>
    /// directory name inside the root where 
    /// </summary>
    public static String CC_MANIFEST_ID = "CC_MANIFEST_ID";
    public static String CC_EVENT_NUMBER = "CC_EVENT_NUMBER";
    public static String CC_EVENT_ID = "CC_EVENT_ID";
    /// <summary>
    /// root path, or bucket name
    /// </summary>
    public static String CC_ROOT = "CC_ROOT"; 

    public static String CC_PLUGIN_DEFINITION = "CC_PLUGIN_DEFINITION"; // becomes prefix for log messages
    /// <summary>
    /// prefix to AWS_* environment variables  to define connection/credientials
    /// </summary>
    public static String CC_PROFILE = "CC"; 

    /// <summary>
    /// pretty print 
    /// </summary>
    public static String CC_PAYLOAD_FORMATTED = "CC_PAYLOAD_FORMATTED";
    public static String AWS_ACCESS_KEY_ID = "AWS_ACCESS_KEY_ID";
    public static String AWS_SECRET_ACCESS_KEY = "AWS_SECRET_ACCESS_KEY";
    public static String AWS_DEFAULT_REGION = "AWS_DEFAULT_REGION";
    public static String AWS_S3_BUCKET = "AWS_S3_BUCKET";
  }
}
