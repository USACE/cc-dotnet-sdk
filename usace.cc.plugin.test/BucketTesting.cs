using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace usace.cc.plugin.test
{
  public class BucketTesting
  {

    [Fact]
    public async void CreateBucket()
    {
      var s3Client = BucketUtility.GetS3Client("KARL");
      using (s3Client)
      {
        await BucketUtility.CreateBucketIfNotExists(s3Client, "test7");
      }
    }

    private static string CreateTestData()
    {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < 100; i++)
      {
        sb.Append(i.ToString() + "\n");
      }
      return sb.ToString();
    }
    [Fact]
    public async void ObjectLifeCycle()
    {
      var s3Client = BucketUtility.GetS3Client("KARL");
      var bucketName = "thyroid";
      await BucketUtility.CreateBucketIfNotExists(s3Client, bucketName);
      using (s3Client)
      {
        var txt = CreateTestData();
        string key = "test-object.txt";
        var created =await BucketUtility.CreateObject(s3Client, bucketName,key, txt);
        Assert.True(created);

        var objectExists = await BucketUtility.ObjectExists(s3Client, bucketName, key);
        Assert.True(objectExists, "object does not exist");

        var txt2 = await BucketUtility.ReadObjectAsText(s3Client, bucketName, key);
        Assert.True(string.Equals(txt, txt2),"content different");


        var dir = Path.Combine(Path.GetTempPath(), "karl-sub-dir-test");
        var locaFileName = Path.Combine(dir, "karl.txt");
        if( File.Exists(locaFileName))
           File.Delete(locaFileName);
        System.Console.WriteLine("saving to local file: "+locaFileName);

        await BucketUtility.SaveObjectToLocalFile(s3Client, bucketName, key, locaFileName);

        Assert.True(File.Exists(locaFileName));

        var deleted = await BucketUtility.DeleteObject(s3Client, bucketName, key);
        Assert.True(deleted, "object was not deleted: " + key);
        bool exists = await BucketUtility.ObjectExists(s3Client, bucketName, key); 
        Assert.False(exists," object not deleted?");

        await BucketUtility.DeleteBucket(s3Client, bucketName);

        exists = await BucketUtility.BucketExists(s3Client, bucketName);
      }

    }
  }
}
