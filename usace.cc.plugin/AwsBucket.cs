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
  /// <summary>
  /// Manage connection and functionality to a S3 bucket
  /// </summary>
  public class AwsBucket:IDisposable
  {
    AmazonS3Client s3Client;
    string bucketName;

    public AwsBucket(string profileName)
    {
      AWSConfig cfg = new AWSConfig(profileName);
      s3Client = GetS3Client(cfg);
      bucketName = cfg.aws_bucket;
    }
    public AwsBucket(string profileName, string bucketName)
    {
      AWSConfig cfg = new AWSConfig(profileName);
      s3Client = GetS3Client(cfg);
      this.bucketName = bucketName;
    }
    private static AmazonS3Client GetS3Client(AWSConfig cfg)
    {
      var awsConfig = new AmazonS3Config()
      {
        ForcePathStyle = true,
      };

      if(cfg.aws_mock)
      {
        awsConfig.ServiceURL = cfg.aws_endpoint;
      }
      else {
        awsConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(cfg.aws_region);
      }

      var s3Client = new AmazonS3Client(cfg.aws_access_key_id,
                           cfg.aws_secret_access_key_id, awsConfig);
      return s3Client;

    }

    public async Task<bool> CreateBucketIfNotExists()
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

    public async Task<bool> DeleteObject(string key)
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
      catch (AmazonS3Exception e)
      {
        Console.WriteLine(e.Message);
        return false;
      }
    }

    internal async Task<bool> CreateObjectFromLocalFile(string objectKey, string fileName)
    {
      var bytes = File.ReadAllBytes(fileName);
      return await CreateObject(objectKey, bytes);
    }

    public async Task<bool> CreateObject(string objectKey, string data)
    {
      var buff = Encoding.UTF8.GetBytes(data);
      MemoryStream stream = new MemoryStream(buff);
      return await CreateObject(objectKey, stream);
    }



    public async Task<bool> CreateObject(string objectKey, Stream stream)
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
    public async Task<bool> CreateObject(string objectKey, byte[] data)
    {
      return await CreateObject(objectKey, new MemoryStream(data));
    }

    public async Task<Byte[]> ReadObjectAsBytes(string key)
    {
      var stream = await ReadObjectAsStream(key);
      using (var memoryStream = new MemoryStream())
      {
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
      }
    }

    public async Task<bool> SaveObjectToLocalFile(string key, string localFileName)
    {
      try
      {
        var dir = Path.GetDirectoryName(localFileName);
        Directory.CreateDirectory(dir);
        var bytes = await ReadObjectAsBytes(key);
        File.WriteAllBytes(localFileName, bytes);
        return true;
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
      return false;

    }

    public async Task<Stream> ReadObjectAsStream(string key)
    {
      var getObjectRequest = new GetObjectRequest
      {
        BucketName = bucketName,
        Key = key
      };

      var getObjectResponse = await s3Client.GetObjectAsync(getObjectRequest);

      return getObjectResponse.ResponseStream;
    }
    public async Task<string> ReadObjectAsText(string key)
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

    public async Task<bool> ObjectExists(string key)
    {
      try
      {
        var request = new GetObjectMetadataRequest
        {
          BucketName = bucketName,
          Key = key
        };

        await s3Client.GetObjectMetadataAsync(request);

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


    public async Task<bool> BucketExists()
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


    public async Task<bool> DeleteBucket()
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

    public void Dispose()
    {
     
    }
  }
}
