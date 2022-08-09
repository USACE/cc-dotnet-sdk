using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Internal;
using Amazon.S3.Model;
using static usace.wat.plugin.Message;

namespace usace.wat.plugin
{
    public sealed class Utilities
    {
        public Config Config { get; private set; }
        public Dictionary<string, IAmazonS3> Clients { get; private set; }
        public bool HasInitialized { get; private set; }
        public Level LogLevel { get; private set; } // need to adjust this set
        public Utilities Instance { get; } = new Utilities();

        public Utilities()
        {
            Clients = new Dictionary<string, IAmazonS3>();
        }
        public static void Initalize()
        {
            InitalizeFromPath("config.json");
        }
        public static void InitializeFromPath(string path)
        {
            Config cfg = new Config();
            FileStream file = File.OpenRead(path);

            //ObjectMapper
            try
            {
                cfg = mapper.readValue(file, Config.GetType());
            }
            catch (Exception)
            {
                Message message = Message.BuildMessage()

                throw;
            }
        }
        public static void Initialize(Config config)
        {
            Instance.Config = config;
            foreach (AWSConfig awsConfig in config.aws_configs)
            {
                AddS3Bucket(awsConfig);
            }
            Instance.setHasInitalized(true);
        }
        private static void AddS3Bucket(AWSConfig awsconfig)
        {
            Amazon.S3.
            S3Region clientRegion = S3Region.FindValue(awsconfig.aws_region);
            try
            {
                AmazonS3Client s3Client = null;
                if (awsconfig.aws_mock)
                {
                    AWSCredentials credentials = new BasicAWSCredentials(awsconfig.aws_access_key_id, awsconfig.aws_secret_access_key_id);
                    
                    ClientConfiguration clientConfiguration = new ClientConfiguration();
                    clientConfiguration.setSignerOverride("AWSS3V4SignerType");

                    s3Client = AmazonS3ClientBuilder
                        .standard()
                        .withEndpointConfiguration(new AwsClientBuilder.EndpointConfiguration(awsconfig.aws_endpoint, clientRegion.name()))
                        .withPathStyleAccessEnabled(awsconfig.aws_force_path_style)
                        .withClientConfiguration(clientConfiguration)
                        .withCredentials(new AWSStaticCredentialsProvider(credentials))
                        .build();
                }
                else
                {
                    AWSCredentials credentials = new BasicAWSCredentials(awsconfig.aws_access_key_id, awsconfig.aws_secret_access_key_id);
                    s3Client = AmazonS3ClientBuilder
                        .standard()
                        .withRegion(clientRegion)
                        .withCredentials(new AWSStaticCredentialsProvider(credentials))
                        .build();
                }
                Instance.setClient(awsconfig.aws_bucket, s3Client);
            }
            catch (AmazonServiceException e)
            {
                // The call was transmitted successfully, but Amazon S3 couldn't process 
                // it, so it returned an error response.
                throw e;
            }
        }
        //https://github.com/awsdocs/aws-doc-sdk-examples/blob/main/dotnetv3/S3/UploadObjectExample/UploadObject.cs
        public static void UploadToS3(string bucketName, string objectKey, byte[] fileBytes)
        {
            try
            {
                //File file = new File(objectPath);
                Stream stream = new MemoryStream(fileBytes);
                AmazonS3Metadata meta = new AmazonS3Metadata();
                PutObjectRequest putOb = new PutObjectRequest();
                putOb.BucketName = bucketName; 
                putOb.Key = objectKey;
                PutObjectResponse response = Instance.getClient(bucketName).putObject(putOb);
                Console.WriteLine(response.ETag);
            }
            catch (AmazonServiceException e)
            {
                Console.WriteLine(e.Message);
            }
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
