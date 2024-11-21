using Amazon.S3;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace Usace.CC.Plugin.Test
{
    public class BucketTesting
    {

        private static void SetEnv(string name, string value)
        {
            Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Process);
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
            string profileName = "JACK";
            SetEnv(profileName + "_" + EnvironmentVariables.AWS_S3_BUCKET, "test-bucket-1");
            SetEnv(profileName + "_" + EnvironmentVariables.AWS_ACCESS_KEY_ID, "USERNAME");
            SetEnv(profileName + "_" + EnvironmentVariables.AWS_SECRET_ACCESS_KEY, "PASSWORD");
            SetEnv(profileName + "_" + "S3_MOCK", "true");
            SetEnv(profileName + "_" + "S3_ENDPOINT", "http://127.0.0.1:9000");

            using var bucket = new AwsBucket(profileName);

            await bucket.CreateBucketIfNotExists();
            using (bucket)
            {
                var txt = CreateTestData();
                string key = "test-object.txt";
                var created = await bucket.CreateObject(key, txt);
                Assert.True(created);

                var objectExists = await bucket.ObjectExists(key);
                Assert.True(objectExists, "object does not exist");

                var txt2 = await bucket.ReadObjectAsText(key);
                Assert.True(string.Equals(txt, txt2), "content different");


                var dir = Path.Combine(Path.GetTempPath(), "jack-sub-dir-test");
                var locaFileName = Path.Combine(dir, "jack.txt");
                if (File.Exists(locaFileName))
                    File.Delete(locaFileName);
                System.Console.WriteLine("saving to local file: " + locaFileName);

                await bucket.SaveObjectToLocalFile(key, locaFileName);

                Assert.True(File.Exists(locaFileName));

                var deleted = await bucket.DeleteObject(key);
                Assert.True(deleted, "object was not deleted: " + key);
                bool exists = await bucket.ObjectExists(key);
                Assert.False(exists, " object not deleted?");

                await bucket.DeleteBucket();

                exists = await bucket.BucketExists();
                Assert.False(exists);
            }
        }
    }
}
