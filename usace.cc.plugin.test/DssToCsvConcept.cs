using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usace.CC.Plugin.Test
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
    
    
    private static void SimulateCC()
    {
      TestUtility.SetEnv(EnvironmentVariables.CC_MANIFEST_ID, "1");
      TestUtility.SetEnv(EnvironmentVariables.CC_EVENT_NUMBER, "987");
      TestUtility.SetEnv(EnvironmentVariables.CC_EVENT_ID, "57");
      TestUtility.SetEnv(EnvironmentVariables.CC_ROOT, "data");
      TestUtility.SetEnv(EnvironmentVariables.CC_PLUGIN_DEFINITION, "dss-to-csv");
      TestUtility.SetEnv(EnvironmentVariables.CC_PROFILE, "CC");

      TestUtility.SetEnv("KARL_PROFILE", "KARL");

      string ccBucketName = "cc-bucket";
      TestUtility.SetEnv(EnvironmentVariables.CC_PROFILE + "_" + EnvironmentVariables.AWS_S3_BUCKET,ccBucketName);
      //TestUtility.CreateBucket(EnvironmentVariables.CC_PROFILE);
      
      //..await bucket.CreateBucketIfNotExists();

      // path to payload:  $CC_ROOT/$CC_MANIFEST_ID/payload
      var cc_root = Environment.GetEnvironmentVariable(EnvironmentVariables.CC_ROOT);
      var cc_profile = Environment.GetEnvironmentVariable(EnvironmentVariables.CC_PROFILE);
      var cc_manifest_id = Environment.GetEnvironmentVariable(EnvironmentVariables.CC_MANIFEST_ID);

      string key = Path.Join(cc_root, cc_manifest_id, Constants.PayloadFileName);
      string data = TestUtility.GetResourceAsText("Usace.CC.Plugin.Test.payload-dss-to-csv.json");

      TestUtility.CreateBucket(cc_profile);
      TestUtility.UploadPayloadFile(EnvironmentVariables.CC_PROFILE, key, data);

    }

    public static async Task Main(string[] args)
    {
      SimulateCC();

      var pm = await PluginManager.CreateAsync();
      pm.LogMessage("hello from DssToCsv Plugin");
      var eventNumber = pm.EventNumber();
      pm.LogMessage("event number is: " + eventNumber);
      var payload = pm.Payload;

      if( ValidPaload(pm, payload))
      {
        // write DSS name, record name, csv name
        var paths = payload.Inputs[0].Paths;
        pm.LogMessage("opening DSS file: '" +paths[0]+"'");
        pm.LogMessage("reading record with dsspath (key): '"+paths[1]+"'");
        pm.LogMessage("Converting to CSV file: '"+payload.Outputs[0].Paths[0]+"'");
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
  }
}
