using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usace.cc.plugin
{
  /// <summary>
  /// CCStore3 is a special Store used by plugins to pull payloads.
  /// 
  /// 
  /// </summary>
  internal class CcStoreS3 : CcStore
  {
    AwsBucket bucket;
    string manifestId;
    string root;
    public CcStoreS3()
    {
     
      var profileName = Utility.GetEnv(EnvironmentVariables.CC_PROFILE);
      bucket = new AwsBucket(profileName);
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
          bucket.CreateObjectFromLocalFile(path, path)).GetAwaiter().GetResult();
        return rval;
      }
      if (input.ObjectState == ObjectState.Memory)
      {
        var rval = Task.Run(() =>
          bucket.CreateObject(path, input.Data)).GetAwaiter().GetResult();
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
          bucket.SaveObjectToLocalFile(path,localPath)).GetAwaiter().GetResult();

      return rval;
    }

    public byte[] GetObject(GetObjectInput input)
    {
      var path = S3Path(input.FileName, input.FileExtension);
      var rval = Task.Run(() =>
        bucket.ReadObjectAsBytes(path)).GetAwaiter().GetResult();
      return rval;
    }


    public string RootPath()
    {
      return bucket.Name;
    }

    /// <summary>
    /// Reads json payload from S3
    /// </summary>
    /// <returns></returns>
    public Payload GetPayload()
    {
      var key = Path.Combine(root,manifestId,Constants.PayloadFileName);

      var json = Task.Run(() =>
        bucket.ReadObjectAsText(key)).GetAwaiter().GetResult();

      Payload p = Payload.FromJson(json);
      return p;
    }

  }
}
