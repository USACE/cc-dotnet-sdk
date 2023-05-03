using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Usace.CC.Plugin.Test
{
  internal class TestUtility
  {
    /// <summary>
    /// SetEnv used to set env variables for testing
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    internal static void SetEnv(string name, string value)
    {
      Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Process);
    }

    internal static bool CreateBucket(string profileName)
    {
      AwsBucket bucket = new AwsBucket(profileName);
      var rval = Task.Run(() =>
       bucket.CreateBucketIfNotExists()).GetAwaiter().GetResult();
      return rval;
    }

    internal static bool UploadPayloadFile(string profileName, string key, string data)
    {
      AwsBucket bucket = new AwsBucket(profileName);
      var rval = Task.Run(() =>
          bucket.CreateObject(key, data)).GetAwaiter().GetResult();
      return rval;
    }
    internal static string GetResourceAsText(string resourceName)
    {
      var assembly = Assembly.GetExecutingAssembly();

      using (var stream = assembly.GetManifestResourceStream(resourceName))
      {
        if (stream == null)
        {
          throw new Exception($"Unable to find resource '{resourceName}' in assembly '{assembly.FullName}'.");
        }

        using (var reader = new StreamReader(stream))
        {
          var contents = reader.ReadToEnd();
          return contents;
        }
      }
    }
  }
}
