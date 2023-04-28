using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usace.cc.plugin
{
  /// <summary>
  /// CCStore is a special Store used by plugins to pull payloads.
  /// 
  /// 
  /// </summary>
  internal class CcStoreS3 : CcStore
  {

    AWSConfig config;
    AmazonS3Client awsS3;
    string manifestId;
    string root;
    public CcStoreS3()
    {
      var profileName = Utility.GetEnv(EnvironmentVariables.CC_PROFILE);
      config = new AWSConfig(profileName);
      awsS3 = BucketUtility.GetS3Client(profileName);
      manifestId = Utility.GetEnv(EnvironmentVariables.CC_MANIFEST_ID);
      root = Utility.GetEnv(EnvironmentVariables.CC_ROOT);
    }

    private string S3Path(string fileName,string extension)
    {
      return Path.Combine(root, manifestId, fileName + "." + extension);
    }
    private string LocalPath(PullObjectInput input)
    {
      return Path.Combine(input.DestRootPath, input.FileName + "." + input.FileExtension);
    }
    public byte[] GetObject(GetObjectInput input)
    {
      throw new NotImplementedException();
    }

    public bool HandlesDataStoreType(StoreType datastoretype)
    {
      return datastoretype == StoreType.S3;
    }


    /// <summary>
    /// Copies a local file, or byte[] to S3
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public bool PutObject(PutObjectInput input)
    {
      var path = S3Path(input.FileName,input.FileExtension);
      if (input.ObjectState == ObjectState.LocalDisk)
      {
        var rval = Task.Run(() =>
          BucketUtility.CreateObjectFromLocalFile(awsS3, config.aws_bucket, path, path)).GetAwaiter().GetResult();
        return rval;
      }
      if (input.ObjectState == ObjectState.Memory)
      {
        var rval = Task.Run(() =>
          BucketUtility.CreateObject(awsS3, config.aws_bucket, path, input.Data)).GetAwaiter().GetResult();
        return rval;
      }

      return false;
    }
    /// <summary>
    /// Copies S3 object to a local file
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public bool PullObject(PullObjectInput input)
    {
      var path = S3Path(input.FileName, input.FileExtension);
      var localPath = LocalPath(input);

      var rval = Task.Run(() =>
          BucketUtility.SaveObjectToLocalFile(awsS3, config.aws_bucket, path,localPath)).GetAwaiter().GetResult();

      return rval;
    }

    public string RootPath()
    {
      throw new NotImplementedException();
    }

    internal Payload GetPayload()
    {
      Console.WriteLine("GetPayload - not implemented");
      return new Payload();
    }

    Payload CcStore.GetPayload()
    {
      throw new NotImplementedException();
    }
  }
}
