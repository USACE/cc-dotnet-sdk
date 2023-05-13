namespace Usace.CC.Plugin
{
  /// <summary>
  /// simple CC similator, used for testing
  /// </summary>
  public class CCSimulator
  {
    /// <summary>
    /// Sets environment variables, creates CC bucket (if needed) and places payload in the bucket
    /// </summary>
    public static async Task Setup(string jsonPayload,string manifestID="1",string eventNumber="987",
                                 string eventID="57",string root="data",
                                 string pluginDefinition="dss-to-csv", string profile="CC",
                                 string ccBucketName="cc-bucket", string userProfile = "karl")
    {
      SetEnv(EnvironmentVariables.CC_MANIFEST_ID, manifestID);
      SetEnv(EnvironmentVariables.CC_EVENT_NUMBER, eventNumber);
      SetEnv(EnvironmentVariables.CC_EVENT_ID, eventID);
      SetEnv(EnvironmentVariables.CC_ROOT, root);
      SetEnv(EnvironmentVariables.CC_PLUGIN_DEFINITION, pluginDefinition);
      SetEnv(EnvironmentVariables.CC_PROFILE, profile);


      SetEnv(EnvironmentVariables.CC_PROFILE + "_" + EnvironmentVariables.AWS_S3_BUCKET, ccBucketName);

      // path to payload:  $CC_ROOT/$CC_MANIFEST_ID/payload
      var cc_root = Environment.GetEnvironmentVariable(EnvironmentVariables.CC_ROOT);
      var cc_profile = Environment.GetEnvironmentVariable(EnvironmentVariables.CC_PROFILE);
      var cc_manifest_id = Environment.GetEnvironmentVariable(EnvironmentVariables.CC_MANIFEST_ID);

      string key = Path.Join(cc_root, cc_manifest_id, Constants.PayloadFileName);
      //string data = TestUtility.GetResourceAsText("Usace.CC.Plugin.Test.payload-dss-to-csv.json");

      await CreateBucket(cc_profile);
      await UploadPayloadFile(EnvironmentVariables.CC_PROFILE, key, jsonPayload);

      await CreateBucket(userProfile);

    }
    private static void SetEnv(string name, string value)
    {
      Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Process);
    }
    private static async Task<bool> CreateBucket(string profileName)
    {
      AwsBucket bucket = new AwsBucket(profileName);
      var rval = await bucket.CreateBucketIfNotExists();
      return rval;
    }

    private static async Task<bool> UploadPayloadFile(string profileName, string key, string data)
    {
      AwsBucket bucket = new AwsBucket(profileName);
      var rval = await bucket.CreateObject(key, data);
      return rval;
    }
    
  }
}
