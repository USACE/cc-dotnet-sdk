namespace Usace.CC.Plugin
{

  /// <summary>
  /// A IFileDataStore that uses S3 storage
  /// 
  /// The object Key for the S3-API calls (Get, Put, Delete, and Copy) is computed 
  /// with a prefix read from the DataStore.Parameters['root']
  /// 
  /// For example: if you  call Delete(path)
  /// the actual S3 call is Delete(prefix+/"+path)
  /// 
  /// </summary>
  public class FileDataStoreS3 : IFileDataStore
  {
    AwsBucket bucket;
    private static String S3ROOT = "root";
    string prefix = "";

    private string GetObjectKey(string path)
    {
            path = path.TrimStart("/");//paths combine adds path separator if it is not present at the *end* of an argument. Does not check the beginning of an argument.
      var key = Path.Combine(prefix,path);
            key = key.TrimStart("/");
      return key;
    }

    public FileDataStoreS3(DataStore ds)
    {
      if( !ds.Parameters.TryGetValue(S3ROOT, out prefix))
      {
        throw new Exception("The DataStore Parameters did not contain the key [" + S3ROOT + "]");
      }
      prefix = prefix.TrimStart('/');
      bucket = new AwsBucket(ds.DsProfile);
    }

    public async Task<bool> Copy(IFileDataStore destStore, string srcPath, string destPath)
    {
      try
      {
        var data = await GetObjectBytes(srcPath);
        MemoryStream ms = new MemoryStream(data);
        return await destStore.Put(ms, destPath);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        return false;
      }
    }

    private async Task<byte[]> GetObjectBytes(string srcPath)
    {
      return await bucket.ReadObjectAsBytes(GetObjectKey(srcPath));
    }

    public async Task<bool> Delete(string path)
    {
      return await bucket.DeleteObject(GetObjectKey(path));
    }
    
    public async Task<bool> PutAsync(Stream data, string path)
    {
      return await bucket.CreateObject(GetObjectKey(path), data);
    }
    public async Task<bool> Put(Stream data, string path)
    {
            var key = GetObjectKey(path);
            Console.WriteLine("Putting object at key: " + key);
      return await bucket.CreateObject(GetObjectKey(path), data);
    }

    public async Task<Stream> Get(string path)
    {
      return await bucket.ReadObjectAsStream(GetObjectKey(path));
    }
  }
}
