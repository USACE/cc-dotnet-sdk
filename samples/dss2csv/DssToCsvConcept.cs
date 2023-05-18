using System.Net;
using System.Reflection;
using Usace.CC.Plugin;

namespace dss2csv
{
   /// <summary>
   /// 
   /// This is a proof of concept file conversion plugin for Clould Compute (CC)
   /// This plugin simulates converting data-sets from DSS to csv 
   /// 
   /// Cloud Compute creates environment variables and payloads, 
   /// and sends these to plugins
   ///
   /// A Plugin does the following steps:
   /// Reads the environment variables, and json file with the payload
   /// 
   /// A developer manually creates a manifest. 
   /// manifest describes:
   ///     compute environment: CPU/ memory/disk requirements
   ///     docker image name
   ///     plugin friendly name
   ///     environment variable overrides
   ///     How to connect/mount block stores EBS
   ///     payload contains:   (plugins compute from info in a payload)
   ///         - stores
   ///         - attributes
   ///         - inputs
   ///         - outputs
   /// </summary>

   internal class DssToCsvConcept
  {

    //  KARL/testing/data/input-data/file.dss
    //  KARL/testing/data/987/output-data/csvfile.csv

    //  CC/data/1/payload.json         // write permissions by CC only; readonly for plugins
    //  $CC_ROOT/$CC_MANIFEST_ID/payload


    public static async Task Main(string[] args)
    {
         /*
KARL_AWS_ACCESS_KEY_ID=mlbDUcvzWyoyd3gy
KARL_AWS_DEFAULT_REGION=us-west-2
KARL_AWS_S3_BUCKET=karl
KARL_AWS_SECRET_ACCESS_KEY=BYj3wYldnVqBZoDcuVgq1LQYHzEgZPmR
KARL_S3_ENDPOINT=http://192.168.206.129:9000
KARL_S3_MOCK=True
         */
       //  CCSimulator.SetBucketVariables(profile: "CC", accessKey:"password",
      //   region:"us-west-2",bucketName:"cc",accessSecret:"",endPoint: "http://192.168.206.129:9000")

      string json = GetResourceAsText("dss2csv.payload-dss-to-csv.json");
         await CCSimulator.Setup(json, manifestID: "1", eventNumber: "987",
                     eventID: "57", root: "data", pluginDefinition: "dss-to-csv",
                     profile: "CC", ccBucketName: "cc-bucket");

       


      var pm = await PluginManager.CreateAsync();
      pm.LogMessage("hello from DssToCsv Plugin");
      var eventNumber = pm.EventNumber();
      pm.LogMessage("event number is: " + eventNumber);
      var payload = pm.Payload;

      if (ValidPaload(pm, payload))
      {
        // write DSS name, record name, csv name
        var paths = payload.Inputs[0].Paths;
        pm.LogMessage("opening DSS file: '" + paths[0] + "'");
        pm.LogMessage("reading record with dsspath (key): '" + paths[1] + "'");
        pm.LogMessage("Converting to CSV file: '" + payload.Outputs[0].Paths[0] + "'");
      }


    }

    private static bool ValidPaload(PluginManager pm, Payload payload)
    {
      if (payload.Inputs.Length != 1)
      {
        pm.LogMessage(new Message("expected one input. Found " + payload.Inputs.Length + " inputs"));
        pm.ReportProgress(new Status(Status.StatusLevel.FAILED, 0));
        return false;
      }
      if (int.TryParse(payload.Attributes["record_count"], out int record_count))
      {
        if (payload.Inputs[0].Paths.Length != 1 + record_count)
        {
          pm.LogMessage(new Message("record_count and Path count not consistent. "));
          pm.ReportProgress(new Status(Status.StatusLevel.FAILED, 0));
          return false;
        }
      }
      else
      {
        pm.LogMessage(new Message("error parsing record_count to an integer"));
        pm.ReportProgress(new Status(Status.StatusLevel.FAILED, 0));
        return false;
      }
      if (payload.Inputs[0].Paths.Length != 2)
      {
        pm.LogMessage(new Message("expected one input. Found " + payload.Inputs.Length + " inputs"));
        pm.ReportProgress(new Status(Status.StatusLevel.FAILED, 0));
        return false;
      }
      if (payload.Outputs.Length != 1)
      {
        pm.LogMessage(new Message("expected one output. Found " + payload.Outputs.Length + " outputs"));
        pm.ReportProgress(new Status(Status.StatusLevel.FAILED, 0));
        return false;
      }
      return true;
    }

    private static string GetResourceAsText(string resourceName)
    {
      var assembly = Assembly.GetExecutingAssembly();

      using (var stream = assembly.GetManifestResourceStream(resourceName))
      {
        if (stream == null)
        {
          throw new Exception($"Unable to find resource '{resourceName}' in assembly '{assembly.FullName}'.");
        }

        using (var reader = new StreamReader(stream))
        {
          var contents = reader.ReadToEnd();
          return contents;
        }
      }
    }
  }
}
