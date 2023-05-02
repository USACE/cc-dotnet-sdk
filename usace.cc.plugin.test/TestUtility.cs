using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    internal async static void CreateBucket(string profileName)
    {
      // create CC bucket
      AwsBucket bucket = new AwsBucket(EnvironmentVariables.CC_PROFILE);
      await bucket.CreateBucketIfNotExists();

    }
  }
}
