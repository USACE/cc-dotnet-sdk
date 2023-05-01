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
      var bucket = new AwsBucket("KARL","test-bucket-983556");
      using (bucket)
      {
        await bucket.CreateBucketIfNotExists();
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
      AwsBucket bucket = new AwsBucket("KARL", "thyroid");

      await bucket.CreateBucketIfNotExists();
      using (bucket)
      {
        var txt = CreateTestData();
        string key = "test-object.txt";
        var created =await bucket.CreateObject(key, txt);
        Assert.True(created);

        var objectExists = await bucket.ObjectExists(key);
        Assert.True(objectExists, "object does not exist");

        var txt2 = await bucket.ReadObjectAsText(key);
        Assert.True(string.Equals(txt, txt2),"content different");


        var dir = Path.Combine(Path.GetTempPath(), "karl-sub-dir-test");
        var locaFileName = Path.Combine(dir, "karl.txt");
        if( File.Exists(locaFileName))
           File.Delete(locaFileName);
        System.Console.WriteLine("saving to local file: "+locaFileName);

        await bucket.SaveObjectToLocalFile(key, locaFileName);

        Assert.True(File.Exists(locaFileName));

        var deleted = await bucket.DeleteObject(key);
        Assert.True(deleted, "object was not deleted: " + key);
        bool exists = await bucket.ObjectExists(key); 
        Assert.False(exists," object not deleted?");

        await bucket.DeleteBucket();

        exists = await bucket.BucketExists();
      }

    }
  }
}
