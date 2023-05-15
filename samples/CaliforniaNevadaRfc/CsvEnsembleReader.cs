namespace Usace.CaliforniaNevadaRfc
{
  public class CsvEnsembleReader 
  {

    string path; // path to csv files
    public CsvEnsembleReader(string path)
    {
      this.path = path;
    }

    // Allegedly threadsafe
    // https://docs.microsoft.com/en-us/dotnet/api/system.console?redirectedfrom=MSDN&view=netframework-4.8
    static bool DebugMode = false;
    static void Log(string msg)
    {
      if (DebugMode)
        Console.WriteLine(msg);
    }
    static void LogWarning(string msg) => Console.WriteLine("Warning: " + msg);
    /// <summary>
    /// Reads list of Forecast
    /// </summary>
    /// <param name="watershedName"></param>
    /// <param name="issueDate"></param>
    /// <returns></returns>
    MultiLocationRfcCsvFile Read(string watershedName, DateTime issueDate)
    {
      //https://www.cnrfc.noaa.gov/csv/2019092312_RussianNapa_hefs_csv_hourly.zip

      string fileName = issueDate.ToString("yyyyMMddhh") + "_";
      fileName += watershedName;
      fileName += "_hefs_csv_hourly";

      string csvFileName = Path.Combine(path, fileName + ".csv");
      if (File.Exists(csvFileName))
      {
        Log("Found " + csvFileName + " in cache.  Reading...");

        MultiLocationRfcCsvFile csv = new MultiLocationRfcCsvFile(csvFileName);

        return csv;

      }
      else
      {
        LogWarning("Warning: " + csvFileName + " not found, skipping");
        return null;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="watershedName"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="locationFilter">location to inlcude (if empty return all locations) </param>
    /// <returns></returns>
    public Watershed Read(string watershedName, DateTime startDate, DateTime endDate,string locationFilter="")
    {

      if (!ValidDates(startDate, endDate))
        return null;
      var output = new Watershed(watershedName);

      DateTime t = startDate;

      while(t <=endDate)
      {

        // Seems threadsafe at a glance
        var csv = Read(watershedName, t);
        if (csv != null)
        {
          foreach (string locName in csv.LocationNames)
          {
            if( locationFilter != "" && String.Compare(locName, locationFilter, true) != 0)
            {// look for specific location only
              continue;
            }
            Forecast f = output.AddForecast(locName, t, csv.GetEnsemble(locName),csv.TimeStamps);
            f.TimeStamps = csv.TimeStamps;
          }
        }
        t = t.AddDays(1);
      }
      return output;
    }

    public Watershed ReadParallel(string watershedName, DateTime startDate, DateTime endDate)
    {
      if (!ValidDates(startDate, endDate))
        return null;

      var output = new Watershed(watershedName);

      // Each forecast is one day
      int numTotal = (int)Math.Round((endDate - startDate).TotalDays) + 1;

      Parallel.For(0, numTotal, i =>
      {
        DateTime day = startDate.AddDays(i);

        var csv = Read(watershedName, day);

        if (csv != null)
        {
          lock (output)
          {
            foreach (string locName in csv.LocationNames)
            {
              Forecast f = output.AddForecast(locName, day, csv.GetEnsemble(locName),csv.TimeStamps);
              f.TimeStamps = csv.TimeStamps;
            }
          }
        }

      });

      return output;
    }

    bool ValidDates(DateTime startDate, DateTime endDate)
    {
      if (startDate.Hour != 12)
      {
        Console.WriteLine("start time must be 12");
        return false;
      }
      if (endDate.Hour != 12)
      {
        Console.WriteLine("end time must be 12");
        return false;
      }

      if (startDate > endDate)
      {
        Console.WriteLine("end date should be after start date");
        return false;
      }
      return true;
    }
  }
}

