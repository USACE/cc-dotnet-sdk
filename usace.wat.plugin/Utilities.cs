using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Internal;
using Amazon.S3.Model;
using Amazon.S3.Util;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static usace.wat.plugin.Message;

namespace usace.wat.plugin
{
    public sealed class Utilities
    {
        public Config Config { get; private set; }
        public Dictionary<string, IAmazonS3> Clients { get; private set; }
        public bool HasInitialized { get; private set; }
        public Level LogLevel { get; private set; } // need to adjust this set
        public static Utilities Instance { get; } = new Utilities();

        public Utilities()
        {
            Clients = new Dictionary<string, IAmazonS3>();
        }
        /// <summary>
        /// Initializes utilities with the config at path "config.json"
        /// </summary>
        public static void Initalize()
        {
            InitializeFromPath("config.json");
        }
        //https://github.com/aaubry/YamlDotNet/blob/master/YamlDotNet.Samples/DeserializeObjectGraph.cs
        /// <summary>
        /// Initializes utilities with the conifg at the path provided
        /// </summary>
        /// <param name="path"> A path to a yaml file </param>
        public static void InitializeFromPath(string path)
        {
            string fileText = File.ReadAllText(path);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            try
            {
                Config config = deserializer.Deserialize<Config>(fileText);
            }
            catch (Exception e)
            {
                Message message = Message.BuildMessage()
                    .withMessage("Error Parsing Configuration Contents: at path " + path + " with error " + e.Message)
                    .fromSender("Plugin Services")
                    .build();
                Log(message);
            }
        }
        /// <summary>
        /// Adds an S3 bucket for each of the AWS configs present in the Config provided. Sets Instance.HasInitialized to true
        /// </summary>
        /// <param name="config"></param>
        public static void Initialize(Config config)
        {
            Instance.Config = config;
            foreach (AWSConfig awsConfig in config.aws_configs)
            {
                AddS3Bucket(awsConfig);
            }
            Instance.HasInitialized = true;
        }
        //https://docs.aws.amazon.com/AmazonS3/latest/userguide/create-bucket-overview.html#create-bucket-get-location-dotnet
        /// <summary>
        /// Adds an S3 Bucket with the configuration specified
        /// </summary>
        /// <param name="awsconfig"></param>
        private static async void AddS3Bucket(AWSConfig awsconfig)
        {
            string bucketName = awsconfig.Aws_config_name;
            RegionEndpoint bucketRegion = RegionEndpoint.GetBySystemName(awsconfig.Aws_region);
            AmazonS3Client s3Client = new AmazonS3Client(bucketRegion);
            if (!await (AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName)))
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

        //https://github.com/awsdocs/aws-doc-sdk-examples/blob/main/dotnetv3/S3/UploadObjectExample/UploadObject.cs
        //https://docs.aws.amazon.com/AmazonS3/latest/userguide/upload-objects.html
        public static void UploadToS3(string bucketName, string objectKey, byte[] fileBytes)
        {

        }
        public static void Log(Message message)
        {
            if (message.level.CompareTo(Instance.getLogLevel()) >= 0)
            {
                Console.WriteLine(message.ToString());
            }
        }

        //https://github.com/awsdocs/aws-doc-sdk-examples/blob/main/dotnetv3/S3/GetObjectExample/GetObject.cs
        private static byte[] DownloadBytesFromS3(String bucketName, String key)
        {
            S3Object fullObject = null;
            try
            {
                // Get an object and print its contents.
                Message message = Message.BuildMessage()
                .withMessage("Downloading from S3: " + bucketName + "/" + key)
                .withErrorLevel(Level.INFO)
                .fromSender("Plugin Services")
                .build();
                Log(message);

                GetObjectRequest getObjectRequest = new GetObjectRequest();
                getObjectRequest.BucketName = bucketName;
                getObjectRequest.Key = key;

                fullObject = Instance.getClient(bucketName).getObject(getObjectRequest);

                fullObject.Con
                    return fullObject.getObjectContent().readAllBytes();
            }
            catch (Exception e)
            {
                Message message = Message.BuildMessage()
                .withMessage("Error Downloading from S3: " + e.getMessage())
                .withErrorLevel(Level.ERROR)
                .fromSender("Plugin Services")
                .build();
                Log(message);
            }
            finally
            {
                // To ensure that the network connection doesn't remain open, close any open input streams.
                if (fullObject != null)
                {
                    try
                    {
                        fullObject.close();
                    }
                    catch (Exception e)
                    {
                        Message message = Message.BuildMessage()
                        .withMessage("Error Closing S3 object: " + e.Message)
                        .withErrorLevel(Level.ERROR)
                        .fromSender("Plugin Services")
                        .build();
                        Log(message);
                    }
                }
            }
            return null;
        }

    }
}
