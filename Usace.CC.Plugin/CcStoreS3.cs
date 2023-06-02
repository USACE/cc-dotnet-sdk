namespace Usace.CC.Plugin
{
  /// <summary>
  /// CCStore3 is a special Store used by plugins to pull payloads.
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
    public async Task<bool> PutObject(PutObjectInput input)
    {
      var path = S3Path(input.FileName,input.FileExtension);
      if (input.ObjectState == ObjectState.LocalDisk)
      {
        return await bucket.CreateObjectFromLocalFile(path, path);
      }
      if (input.ObjectState == ObjectState.Memory)
      {
        return await bucket.CreateObject(path, input.Data);
      }

      return false;
    }
    /// <summary>
    /// Copies S3 object to a local file
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<bool> PullObject(PullObjectInput input)
    {
      var path = S3Path(input.FileName, input.FileExtension);
      var localPath = LocalPath(input);

      return await bucket.SaveObjectToLocalFile(path, localPath);
    }

    public async Task<byte[]> GetObject(GetObjectInput input)
    {
      var path = S3Path(input.FileName, input.FileExtension);
      return await bucket.ReadObjectAsBytes(path);
    }

    public string RootPath()
    {
      return bucket.Name;
    }

    /// <summary>
    /// Reads json payload from S3
    /// </summary>
    /// <returns></returns>
    public async Task<Payload> GetPayload()
    {
      var key = Path.Combine(root,manifestId,Constants.PayloadFileName);

      var json = await bucket.ReadObjectAsText(key);
      Payload p = Payload.FromJson(json);
      return p;
    }

  }
}
