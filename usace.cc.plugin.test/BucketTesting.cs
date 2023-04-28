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
    private static Payload CreateTestPayload(string storeName)
    {
      var payload = new Payload();
      var id = "6ba7b810-9dad-11d1-80b4-00c04fd430c8";

      var ds = new DataStore()
      {
        ID = id,
        Name = storeName,
        StoreType = StoreType.S3,
        DsProfile = "FFRD",
        Parameters = new Dictionary<string, string>(),
        Session = null
      };
      ds.Parameters.Add("root", "/muncie");
      payload.Stores = new DataStore[] { ds };
      var inputs = new List<DataSource>();
      inputs.Add(new DataSource() {
        Name = "seedgenerator",
        ID = id,
        Paths = new string[] { "seeds/sg.json" },
        StoreName = storeName
      });

      payload.Inputs = inputs.ToArray();

      var output = new List<DataSource>();
      output.Add(new DataSource()
      {
        Name = "seedgenerator",
        ID = id,
        Paths = new string[] { "seeds/sg.json" },
        StoreName = storeName
      });

      payload.Outputs = output.ToArray();
      return payload;
    }

    [Fact]
    public void Test1()
    {
      var payload = CreateTestPayload("FFRD");
      FileDataStoreS3 s3 = new FileDataStoreS3(payload.Stores[0]);
      //Stream s;
     // s3.

    }

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
