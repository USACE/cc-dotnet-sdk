using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System.Text;

namespace usace.cc.plugin
{
  public class BucketUtility
  {
    public static AmazonS3Client GetS3Client(string profileName)
    {
      AWSConfig cfg = new AWSConfig(profileName);

      var awsConfig = new AmazonS3Config()
      {
        ServiceURL = cfg.aws_endpoint,
        ForcePathStyle = true,
      };

      if (!cfg.aws_mock)
      {
        awsConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(cfg.aws_region);
      }

      var s3Client = new AmazonS3Client(cfg.aws_access_key_id,
                           cfg.aws_secret_access_key_id, awsConfig);
      return s3Client;

    }

    public static async Task<bool> CreateBucketIfNotExists(AmazonS3Client s3Client, string bucketName)
    {
      var request = new PutBucketRequest
      {
        BucketName = bucketName,
        UseClientRegion = true // use the same region as the client
      };
      bool exists = await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName);
      if (!exists)
      {
        try
        {
          s3Client.PutBucketAsync(request).Wait();
          return true;
        }
        catch (AmazonS3Exception e)
        {
          Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
        }
      }
      return false;
    }

    public static async Task<bool> DeleteObject(AmazonS3Client s3Client, string bucketName, string key)
    {
      DeleteObjectRequest request = new DeleteObjectRequest
      {
        BucketName = bucketName,
        Key = key
      };
      try
      {
        var rval = await s3Client.DeleteObjectAsync(request);
        return true;
      }
      catch(AmazonS3Exception e)
      {
        Console.WriteLine(e.Message);
        return false;
      }
    }

    public static async Task<bool> CreateObject(AmazonS3Client s3Client, string bucketName,
                                      string objectKey, string data)
    {
      var buff = Encoding.UTF8.GetBytes(data);
      MemoryStream stream = new MemoryStream(buff);
      return await CreateObject(s3Client, bucketName, objectKey, stream);
    }



    public static async Task<bool> CreateObject(AmazonS3Client s3Client, string bucketName,
                                       string objectKey, Stream stream)
    {
      try
      {
        var request = new PutObjectRequest
        {
          BucketName = bucketName,
          Key = objectKey,
          InputStream = stream
        };

        var response = await s3Client.PutObjectAsync(request);
        Console.WriteLine($"Object created. ETag: {response.ETag}");
      }
      catch (AmazonS3Exception e)
      {
        Console.WriteLine($"Error encountered on server. Message: '{e.Message}' when writing an object");
        return false;
      }
      catch (Exception e)
      {
        Console.WriteLine($"Unknown encountered on server. Message: '{e.Message}' when writing an object");
        return false;
      }
      return true;
    }
    public static async Task<bool> CreateObject(AmazonS3Client s3Client, string bucketName,
                                          string objectKey, byte[] data)
    {
     return await CreateObject(s3Client, bucketName, objectKey, new MemoryStream(data));
    }

    public static async Task<Stream> ReadObjectAsStream(AmazonS3Client s3Client, string key, string bucketName)
    {
      var getObjectRequest = new GetObjectRequest
      {
        BucketName = bucketName,
        Key = key
      };

      var getObjectResponse = await s3Client.GetObjectAsync(getObjectRequest);

      return getObjectResponse.ResponseStream;
    }
    public static async Task<string> ReadObjectAsText(AmazonS3Client s3Client, string bucketName, string key)
    {
      var getObjectRequest = new GetObjectRequest
      {
        BucketName = bucketName,
        Key = key
      };

      var response = await s3Client.GetObjectAsync(getObjectRequest);


      using (var reader = new StreamReader(response.ResponseStream))
      {
        string content = await reader.ReadToEndAsync();
        return content;
      }

    }

    public async static Task<bool> ObjectExists(AmazonS3Client client, string bucketName, string key)
    {
      try
      {
        var request = new GetObjectMetadataRequest
        {
          BucketName = bucketName,
          Key = key
        };

        await client.GetObjectMetadataAsync(request);

        return true; // Object exists
      }
      catch (AmazonS3Exception e)
      {
        if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
          return false; // Object does not exist
        }
        else
        {
          throw; // Some other error occurred
        }
      }
    }


    public static async Task<bool> BucketExists(AmazonS3Client s3Client, string bucketName)
    {
      try
      {
        var response = await s3Client.ListBucketsAsync();
        return response.Buckets.Any(b => b.BucketName == bucketName);
      }
      catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
      {
        return false;
      }
    }


    public static async Task<bool> DeleteBucket(AmazonS3Client s3Client, string bucketName)
    {
      try
      {
        await s3Client.DeleteBucketAsync(bucketName);
        return true;
      }
      catch (AmazonS3Exception ex)
      {
        Console.WriteLine($"Error deleting S3 bucket: {ex.Message}");
        return false;
      }
    }



  //private static async Task<byte[]> ReadObjectAsBytes(AWSConfig config, string objectName)
  //{
  //  RegionEndpoint bucketRegion = RegionEndpoint.GetBySystemName(config.aws_region);
  //  IAmazonS3 client = new AmazonS3Client(bucketRegion);
  //  var rval = new byte[0];
  //  try
  //  {
  //    GetObjectRequest request = new GetObjectRequest
  //    {
  //      BucketName = config.aws_bucket,
  //      Key = objectName
  //    };
  //    var size = request.ByteRange.End - request.ByteRange.Start;
  //    using (GetObjectResponse response = await client.GetObjectAsync(request))
  //    {
  //      using (Stream responseStream = response.ResponseStream)
  //      {
  //        rval = Utility.ReadBytes(responseStream);
  //      }
  //    }
  //  }
  //  catch (AmazonS3Exception e)
  //  {
  //    // If bucket or object does not exist
  //    Console.WriteLine("Error encountered ***. Message:'{0}' when reading object", e.Message);
  //  }
  //  catch (Exception e)
  //  {
  //    Console.WriteLine("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
  //  }
  //  return rval;
  //}
}
}
