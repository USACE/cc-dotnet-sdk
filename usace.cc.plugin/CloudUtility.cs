using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace usace.cc.plugin
{
  public class CloudUtility
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

    public static async Task CreateBucketIfNotExists(AmazonS3Client s3Client, string bucketName)
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
        }
        catch (AmazonS3Exception e)
        {
          Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
        }
      }
    }

  }
}
