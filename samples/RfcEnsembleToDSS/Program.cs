using Usace.CC.Plugin;
using Usace.CaliforniaNevadaRfc;
using System.Reflection;

namespace RfcEnsembleToDSS {
  public class Program
  {

    public static async Task Main(string[] args)
    {
      string json = GetResourceAsText("RfcEnsembleToDSS.payload-hefs-to-dss.json");
      await CCSimulator.Setup(json, manifestID: "101", eventNumber: "987",
                  eventID: "57", root: "data", pluginDefinition: "hefs-to-csv",
                  profile: "CC", ccBucketName: "cc-bucket2",userProfile:"karl");


      var pm = await PluginManager.CreateAsync();
      var payload = pm.Payload;
      var location = payload.Attributes["location"];
      var date = payload.Attributes["date"];
      var watershedName = payload.Attributes["watershedName"];

      DateTime t = DateTime.Parse(date);

      var waterShed = EnsembleUtility.ReadEnsembleForecast(watershedName, location, t);

      var dss = DateTime.Now.Ticks + "_temp_file.dss";

      DssEnsemble.Write(dss, waterShed, "https://www.cnrfc.noaa.gov/csv/");
      //DssEnsemble.WriteToTimeSeriesProfiles(dss, waterShed); // not yet implemented.

      //  read payload output specification (dss file )
      //     * create dss file local to container
      //     * for each output create appropriate data-set (timeseries-profile)
      //
      //payload.Stores[0];
      //  push dss file to a data store
      DataSource ds = payload.Outputs[0];
      await pm.PutFile(dss, ds, 0); // -- needs work...

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