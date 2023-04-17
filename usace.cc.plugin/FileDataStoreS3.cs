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
    //private static String S3ROOT = "root";


    public FileDataStoreS3(DataStore ds)
    {
      config = new AWSConfig();
      config.aws_region = Utility.GetEnv(ds.DsProfile + "_" + EnvironmentVariables.AWS_DEFAULT_REGION);
      config.aws_secret_access_key_id = Utility.GetEnv(ds.DsProfile + "_" + EnvironmentVariables.AWS_SECRET_ACCESS_KEY);
      config.aws_region = Utility.GetEnv(ds.DsProfile + "_" + EnvironmentVariables.AWS_DEFAULT_REGION);
      config.aws_bucket = Utility.GetEnv(ds.DsProfile + "_" + EnvironmentVariables.AWS_S3_BUCKET);
      config.aws_mock = Boolean.Parse(Utility.GetEnv(ds.DsProfile + "_" + "S3_MOCK"));//convert to boolean;
      config.aws_endpoint = Utility.GetEnv(ds.DsProfile + "_" + "S3_ENDPOINT");
      config.aws_disable_ssl = Boolean.Parse(Utility.GetEnv(ds.DsProfile + "_" + "S3_DISABLE_SSL"));//convert to bool?
      config.aws_force_path_style = Boolean.Parse(Utility.GetEnv(ds.DsProfile + "_" + "S3_FORCE_PATH_STYLE"));//convert to bool
      AddS3Bucket(config);

    }

   

      private async void AddS3Bucket(AWSConfig config) { 

      string bucketName = config.aws_bucket;
      RegionEndpoint bucketRegion = RegionEndpoint.GetBySystemName(config.aws_region);
      s3Client = new AmazonS3Client(bucketRegion);

      if (config.aws_mock)
      {
        return;
        //AWSCredentials aWSCredentials = new BasicAWSCredentials(acfg.aws_access_key_id, acfg.aws_secret_access_key_id);
        //ClientConfig config = new ClientConfig();
        //config.SignerOverride();
        
        //s3Client = AmazonS3C
       }

      if (!await(AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName)))
      {
        var putBucketRequest = new PutBucketRequest
        {
          BucketName = bucketName,
          UseClientRegion = true
        };
        try
        {
          PutBucketResponse putBucketResponse = await s3Client.PutBucketAsync(putBucketRequest);
        }
        catch (AmazonS3Exception e)
        {
          Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
        }

      }
    }
    public bool Copy(IFileDataStore destStore, string srcPath, string destPath)
    {
      throw new NotImplementedException();
    }

    public bool Delete(string path)
    {
      throw new NotImplementedException();
    }

    //https://stackoverflow.com/questions/34144494/how-can-i-get-the-bytes-of-a-getobjectresponse-from-s3
    private static byte[] ReadStream(Stream responseStream)
    {
      byte[] buffer = new byte[256];
      using (MemoryStream ms = new MemoryStream())
      {
        int read;
        while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
        {
          ms.Write(buffer, 0, read);
        }
        return ms.ToArray();
      }
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
            rval = ReadStream(responseStream);
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
