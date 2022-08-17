using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Internal;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System.Text;
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
        public Level LogLevel { get; private set; }
        public static Utilities Instance { get; } = new Utilities();

        public Utilities()
        {
            Clients = new Dictionary<string, IAmazonS3>();
        }
        public static void Initalize()
        {
            InitializeFromPath("config.json");
        }
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
        public static void Initialize(Config config)
        {
            Instance.Config = config;
            foreach (AWSConfig awsConfig in config.aws_configs)
            {
                AddS3Bucket(awsConfig);
            }
            Instance.HasInitialized = true;
        }
        private static async void AddS3Bucket(AWSConfig awsconfig)
        {
            string bucketName = awsconfig.Aws_config_name;
            RegionEndpoint bucketRegion = RegionEndpoint.GetBySystemName(awsconfig.Aws_region);
            AmazonS3Client s3Client = new AmazonS3Client(bucketRegion);

            if (awsconfig.Aws_mock)
            {
                AWSCredentials aWSCredentials = new BasicAWSCredentials(awsconfig.Aws_access_key_id, awsconfig.Aws_secret_access_key_id);
                ClientConfig config = new();
                config.SignerOverride();

                s3Client = AmazonS3C
            }

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
        public static async Task UploadToS3Async(string bucketName, string objectKey, Stream stream)
        {
            try
            {
                var putRequest2 = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    InputStream = stream
                };
                PutObjectResponse response = await Instance.Clients[bucketName].PutObjectAsync(putRequest2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        public static async Task<Stream?> DownloadBytesFromS3(string bucketName, string key)
        {
            Message message = Message.BuildMessage()
                .withMessage("Downloading from S3: " + bucketName + "/" + key)
                .withErrorLevel(Level.INFO)
                .fromSender("Plugin Services")
                .build();
            Log(message);

            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };
                var client = Instance.Clients[bucketName];
                GetObjectResponse response = await client.GetObjectAsync(request);
                return response.ResponseStream;

            }
            catch (AmazonS3Exception e)
            {
                // If bucket or object does not exist
                Console.WriteLine("Error encountered ***. Message:'{0}' when reading object", e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
                return null;
            }
        }
        private static void writeInputStreamToDisk(Stream input, string outputDestination)
        {
            FileStream fileStream = File.Create(outputDestination);
            input.CopyTo(fileStream);
        }
        private static ModelPayload ReadYamlModelPayloadFromStream(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string yamlAsString = reader.ReadToEnd(); ;
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            ModelPayload payload = deserializer.Deserialize<ModelPayload>(yamlAsString);
            return payload;
        }
        public static async Task<ModelPayload?> LoadPayload(string filepath)
        {
            //use primary s3 bucket to find the payload
            if (!Instance.HasInitialized)
            {
                InitializeFromPath(filepath);
            }
            Message message = Message.BuildMessage()
                .withMessage("reading payload at path: " + filepath)
                .withErrorLevel(Level.INFO)
                .fromSender("Plugin Services")
                .build();
            Log(message);
            ModelPayload payload = new ModelPayload();
            if (Instance.Config.aws_configs.Length == 0)
            {
                Message message2 = Message.BuildMessage()
                .withMessage("Configuration contains no AWS Configurations")
                .withErrorLevel(Level.ERROR)
                .fromSender("Plugin Services")
                .build();
                Log(message2);
                return null;
            }
            AWSConfig config = Instance.Config.PrimaryConfig();
            if (config == null)
            {
                return payload;
            }
            Stream body = await DownloadBytesFromS3(config.Aws_bucket, filepath);
            return ReadYamlModelPayloadFromStream(body);
        }
        public static void Log(Message message)
        {
            if (message.level.CompareTo(Instance.LogLevel) >= 0)
            {
                Console.WriteLine(message.ToString());
            }
        }
        public static async Task<Stream?> DownloadObjectAsync(ResourceInfo info)
        {
            switch (info.Store)
            {
                case StoreTypes.S3:
                    return await DownloadBytesFromS3(info.Root, info.Path);
                case StoreTypes.LOCAL:
                    return null;
                default:
                    return null;
            }
        }
        public static async Task UploadFile(ResourceInfo info, Stream stream)
        {
            switch (info.Store)
            {
                case StoreTypes.S3:
                    await UploadToS3Async(info.Root, info.Path, stream);
                    break;
                case StoreTypes.LOCAL:
                    try
                    {
                        writeInputStreamToDisk(stream, info.Root + Path.PathSeparator + info.Path);
                    }
                    catch (IOException e)
                    {
                        Message message = Message.BuildMessage()
                        .withMessage("Error Uploading local file: " + info.Path + " " + e.Message)
                        .withErrorLevel(Level.ERROR)
                        .fromSender("Plugin Services")
                        .build();
                        Log(message);
                    }
                    break;
                default:
                    Message defaultmessage = Message.BuildMessage()
                        .withMessage("Error Uploading local file to " + info.Store)
                        .withErrorLevel(Level.ERROR)
                        .fromSender("Plugin Services")
                        .build();
                    Log(defaultmessage);
                    break;
            }
        }
        public static async Task<EventConfiguration> LoadEventConfiguration(ResourceInfo resourceInfo)
        {
            Message message = Message.BuildMessage()
                .withMessage("reading event configuration at path: " + resourceInfo.Path)
                .withErrorLevel(Level.INFO)
                .fromSender("Plugin Services")
                .build();
            Log(message);
            if (!Instance.HasInitialized)
            {
                InitializeFromPath("config.json");
            }
            Stream body = await DownloadBytesFromS3(resourceInfo.Root, resourceInfo.Path);

            try
            {
                StreamReader reader = new StreamReader(body);
                string yamlAsString = reader.ReadToEnd(); ;
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                    .Build();
                EventConfiguration config = deserializer.Deserialize<EventConfiguration>(yamlAsString);
                return config;
            }

            catch (Exception e)
            {
                Message message2 = Message.BuildMessage()
                .withMessage("Error Parsing Event Configuration Contents: " + e.Message)
                .withErrorLevel(Level.ERROR)
                .fromSender("Plugin Services")
                .build();
                Log(message2);
            }
            return new EventConfiguration();
        }
    }
}

