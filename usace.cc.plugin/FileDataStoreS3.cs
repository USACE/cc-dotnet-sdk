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
      var key = Path.Combine(prefix,path);
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

    public bool Copy(IFileDataStore destStore, string srcPath, string destPath)
    {
      try
      {
        var data = GetObjectBytes(srcPath);
        MemoryStream ms = new MemoryStream(data);
        return destStore.Put(ms, destPath);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        return false;
      }
    }

    private byte[] GetObjectBytes(string srcPath)
    {
      
      var rval = Task.Run(() =>
         bucket.ReadObjectAsBytes(GetObjectKey(srcPath))).GetAwaiter().GetResult();
      return rval;
    }

    public bool Delete(string path)
    {
      bool rval = Task.Run(() =>
          bucket.DeleteObject(GetObjectKey(path))).GetAwaiter().GetResult();
      return rval;
    }
    public async Task<bool> DeleteAsync(string path)
    {
      var rval = await bucket.DeleteObject(GetObjectKey(path));
      return rval;
    }

    
    public async Task<bool> PutAsync(Stream data, string path)
    {
      return await bucket.CreateObject(GetObjectKey(path), data);
    }
    public bool Put(Stream data, string path)
    {
      bool rval = Task.Run(() =>
          bucket.CreateObject(GetObjectKey(path), data)).GetAwaiter().GetResult();
      return rval;
    }

    public Stream Get(string path)
    {
      var rval = Task.Run(() =>
      bucket.ReadObjectAsStream(GetObjectKey(path))).GetAwaiter().GetResult();
      return rval;
    }
  }
}
