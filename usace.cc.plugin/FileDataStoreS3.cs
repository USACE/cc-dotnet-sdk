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
      s3Client = CloudUtility.GetS3Client(ds.DsProfile);
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
      throw new NotImplementedException();
    }

    public async Task<byte[]> ReadBytes(string objectName)
    {
      RegionEndpoint bucketRegion = RegionEndpoint.GetBySystemName(config.aws_region);
      IAmazonS3 client = new AmazonS3Client(bucketRegion);
      var rval = new byte[0];
      try
      {
        GetObjectRequest request = new GetObjectRequest
        {
          BucketName = config.aws_bucket,
          Key = objectName
        };
        var size = request.ByteRange.End - request.ByteRange.Start;
        using (GetObjectResponse response = await client.GetObjectAsync(request))
        {
          using (Stream responseStream = response.ResponseStream)
          {
            rval = Utility.ReadBytes(responseStream);
          }
        }
      }
      catch (AmazonS3Exception e)
      {
        // If bucket or object does not exist
        Console.WriteLine("Error encountered ***. Message:'{0}' when reading object", e.Message);
      }
      catch (Exception e)
      {
        Console.WriteLine("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
      }
      return rval;
    }
    public bool Put(Stream data, string path)
    {
      throw new NotImplementedException();
    }

    Stream IFileDataStore.Get(string path)
    {
      throw new NotImplementedException();
    }
  }
}
