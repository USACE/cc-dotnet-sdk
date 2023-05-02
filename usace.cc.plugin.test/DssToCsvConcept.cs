using System;
using System.Collections.Generic;
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
  /// Reads the environment variables, if the payload is in a json file
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
    //  $CC_PROFILE/$CC_MANIFEST_ID/payload
    
    
    private static void SimulateCC()
    {
      TestUtility.SetEnv(EnvironmentVariables.CC_MANIFEST_ID, "1");
      TestUtility.SetEnv(EnvironmentVariables.CC_EVENT_NUMBER, "987");
      TestUtility.SetEnv(EnvironmentVariables.CC_EVENT_ID, "57");
      TestUtility.SetEnv(EnvironmentVariables.CC_ROOT, "data");
      TestUtility.SetEnv(EnvironmentVariables.CC_PLUGIN_DEFINITION, "dss-to-csv");
      TestUtility.SetEnv(EnvironmentVariables.CC_PROFILE, "CC");

      TestUtility.SetEnv("KARL_PROFILE", "KARL");

      string ccBucketName = "cc_bucket";
      TestUtility.SetEnv(EnvironmentVariables.CC_PROFILE + "_" + EnvironmentVariables.AWS_S3_BUCKET,ccBucketName);
      TestUtility.CreateBucket(EnvironmentVariables.CC_PROFILE);

     // TestUtility.InsertPayload(EnvironmentVariables.CC_PROFILE, "payload");

      //SetEnv("CC_PAYLOAD_FORMATTED", "True");
    }

    public static void Main(string[] args)
    {
      SimulateCC();

      var pm = new PluginManager();
      pm.LogMessage(new Message("hello from DssToCsv Plugin"));
      var eventNumber = pm.EventNumber();
      pm.LogMessage(new Message("event number is: " + eventNumber));
      var payload = pm.Payload;

      if (payload.Inputs.Length != 1)
      {
        pm.LogMessage(new Message("expected one input. Found " + payload.Inputs.Length + " inputs"));
        pm.ReportProgress(new Status(Status.StatusLevel.FAILED, 0));
        return;
      }
      if(int.TryParse(payload.Attributes["record_count"],out int record_count))
      {
        if (payload.Inputs[0].Paths.Length != 1+record_count)
        {
          pm.LogMessage(new Message("record_count and Path count not consistent. "));
          pm.ReportProgress(new Status(Status.StatusLevel.FAILED, 0));
          return;
        }
      }
      else
      {
        pm.LogMessage(new Message("error parsing record_count to an integer"));
        pm.ReportProgress(new Status(Status.StatusLevel.FAILED, 0));
        return;
      }
      if (payload.Inputs[0].Paths.Length != 1)
      {
        pm.LogMessage(new Message("expected one input. Found " + payload.Inputs.Length + " inputs"));
        pm.ReportProgress(new Status(Status.StatusLevel.FAILED, 0));
        return;
      }
      if (payload.Outputs.Length != 1)
      {
        pm.LogMessage(new Message("expected one output. Found " + payload.Outputs.Length + " outputs"));
        pm.ReportProgress(new Status(Status.StatusLevel.FAILED, 0));
        return;
      }

      // write DSS name, record name, csv name


    }
  }
}
