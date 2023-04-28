using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace usace.cc.plugin
{
  
  public class FileDataStoreS3 : IFileDataStore
  {
    //String bucket;
    //String postFix;
    StoreType storeType;
    AmazonS3Client s3Client;
    AWSConfig config;
    private static String S3ROOT = "root";
    string prefix = "";

    public FileDataStoreS3(DataStore ds)
    {
      if( !ds.Parameters.TryGetValue(S3ROOT, out prefix))
      {
        throw new Exception("The DataStore.Parameters did not contain the key [" + S3ROOT + "]");
      }
      prefix = prefix.TrimStart('/');
      config = new AWSConfig(ds.DsProfile);
      s3Client = BucketUtility.GetS3Client(ds.DsProfile);
    }

    public bool Copy(IFileDataStore destStore, string srcPath, string destPath)
    {
      throw new NotImplementedException();

      //byte[] data;
      //try
      //{
      //  data = CloudUtility.GetObjectBytes(srcPath);

      //  ByteArrayInputStream bias = new ByteArrayInputStream(data);
      //  return destStore.Put(bias, destPath);
      //}
      //catch (RemoteException e)
      //{
      //  e.printStackTrace();
      //  return false;
      //}
    }

    public bool Delete(string path)
    {
      bool rval = Task.Run(() =>
          BucketUtility.DeleteObject(s3Client, config.aws_bucket, path)).GetAwaiter().GetResult();
      return rval;
    }
    public async Task<bool> DeleteAsync(string path)
    {
      var rval = await BucketUtility.DeleteObject(s3Client, config.aws_bucket, path);
      return rval;
    }

    
    public async Task<bool> PutAsync(Stream data, string path)
    {
      var key = Path.Combine(prefix, path);
      return await BucketUtility.CreateObject(s3Client, config.aws_bucket,key, data);
    }
    public bool Put(Stream data, string path)
    {
      bool rval = Task.Run(() =>
          PutAsync(data, path)).GetAwaiter().GetResult();
      return rval;

    }


    Stream IFileDataStore.Get(string path)
    {
      throw new NotImplementedException();
    }
  }
}
