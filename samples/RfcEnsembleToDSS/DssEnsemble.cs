using Hec.Dss;
using Usace.CaliforniaNevadaRfc;

namespace RfcEnsembleToDSS
{
  public class DssEnsemble
  {
    public static void Write(string dssFileName, Watershed watershed, string tag="")
    {
      bool saveAsFloat = true;
      float[] ensembleMember = null;
      int count = 0;
      string versionFMT = "yyyyMMdd-HHmmss";
      string forecastTimeFMT = "yyyyMMdd-HHmm";
      using (var w = new DssWriter(dssFileName))
      {
        foreach (Location loc in watershed.Locations)
        {
          int memberCounter = 0;

          foreach (Forecast f in loc.Forecasts)
          {
            int size = f.Ensemble.GetLength(0);
            for (int i = 0; i < size; i++)
            {
              f.EnsembleMember(i, ref ensembleMember);

              memberCounter++;

                var t = f.IssueDate;
                DateTime version = f.IssueDate.Date.AddHours(9).AddMinutes(50);
               //HELLS CANYON-DAM/FLOW-IN/01Nov2021 - 01May2023/6HOUR/C:001981|T:20211126-1800|V:20211126-1906|NWRFC 180-DAY VIA WEB Scrape/
                string F = "C:" + memberCounter.ToString().PadLeft(6, '0') + "|T:" 
                        + t.ToString(forecastTimeFMT)
                        + "|V:" + version.ToString(versionFMT) + "|tag";

                var path = "/" + watershed.Name.ToString() + "/" + loc.Name + "/FLOW-IN//1Hour/" + F + "/";

                Hec.Dss.TimeSeries timeseries = new Hec.Dss.TimeSeries
                {
                  Values = Array.ConvertAll(ensembleMember, item => (double)item),
                  Units = "kcfs",
                  DataType = "INST-VAL",
                  Path = new DssPath(path),
                  StartDateTime = f.TimeStamps[0]
                };
                w.Write(timeseries, saveAsFloat);
              
              count++;
            }
          }
        }
      }
    }

    /// <summary>
    /// /RUSSIANNAPA/APCC1/ensemble-FLOW/01SEP2019/1HOUR/T:0212019/
    /// </summary>
    /// <param name="dssFileName"></param>
    /// <param name="watersheds"></param>
    /// <param name="saveAsFloat"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static void WriteToTimeSeriesProfiles(string dssFileName, Watershed watershed)
    {
      bool saveAsFloat = true;

      int count = 0;
      double[,] ensemble = null;
      using (var w = new DssWriter(dssFileName))
      {
        foreach (Location loc in watershed.Locations)
        {
          foreach (Forecast f in loc.Forecasts)
          {
            ArrayUtility.TransposeFloatToDouble(f.Ensemble, ref ensemble);

            if (count % 100 == 0)
              Console.Write(".");

            TimeSeriesProfile ts = new TimeSeriesProfile();
            ts.StartDateTime = f.IssueDate;

            //  /RUSSIANNAPA/APCC1/Ensemble-FLOW/01SEP2019/1HOUR/T:0212019/
            string F = "|T:" + f.IssueDate.DayOfYear.ToString().PadLeft(3, '0') + f.IssueDate.Year.ToString();
            var path = "/" + watershed.Name.ToString() + "/" + loc.Name + "/Ensemble-Flow//1Hour/" + F + "/";

            ts.ColumnValues = Array.ConvertAll(Enumerable.Range(1, ensemble.GetLength(1)).ToArray(), x => (double)x);
            ts.DataType = "INST-VAL";
            ts.Path = new DssPath(path);
            ts.Values = ensemble;
            ts.ColumnUnits = "cfs";


            w.Write(ts, saveAsFloat);
            count++;

          }
        }
      }
    }
    public static Watershed Read(string watershedName, DateTime start, DateTime end, string dssPath)
    {
      Watershed rval = new Watershed(watershedName);
     // DssReader.UseTrainingWheels = false;

      using (DssReader dss = new DssReader(dssPath))
      {
        Console.WriteLine("Reading " + dssPath);
        DssPathCollection dssPaths = dss.GetCatalog(); // sorted
        int size = dssPaths.Count;
        if (size == 0)
        {
          throw new Exception("Empty DSS catalog");
        }

        // /RUSSIANNAPA/APCC1/FLOW/01SEP2019/1HOUR/C:000002|T:0212019/

        var seriesList = new List<Hec.Dss.TimeSeries>();
        for (int i = 0; i < size; i++)
        {
          if (i % 100 == 0)
            Console.Write(".");

          DssPath path = dssPaths[i];
          string location = path.Bpart;
          float[,] ensemble = null;
          ParseFPart(path.Fpart, out int memberidx, out DateTime issueDate);

          if (issueDate >= start && issueDate <= end && string.Equals(path.Apart, watershedName, StringComparison.OrdinalIgnoreCase))
          {
            // Passing in 'path' (not the dateless string) is important, path without date triggers a heinous case in the dss low-level code
            var ts = dss.GetTimeSeries(path);

            if (NextForecast(seriesList, ts) || i == size - 1)
            {
              if (i == size - 1)
                seriesList.Add(ts);
              ConvertListToEnsembleArray(seriesList, ref ensemble);
              rval.AddForecast(path.Bpart, issueDate, ensemble, ts.Times);
              seriesList.Clear();
            }
            seriesList.Add(ts);
          }
        }
      }

      return rval;
    }

    private static float[,] ConvertListToEnsembleArray(List<Hec.Dss.TimeSeries> seriesList, ref float[,] data)
    {
      int width = seriesList[0].Values.Length;
      int height = seriesList.Count;
      if (data == null || data.GetLength(0) != height || data.GetLength(1) != width)
        data = new float[height, width];

      for (int r = 0; r < height; r++)
      {
        var vals = seriesList[r].Values;
        for (int c = 0; c < width; c++)
        {
          data[r, c] = (float)vals[c];
        }
      }

      return data;
    }

    private static bool NextForecast(List<Hec.Dss.TimeSeries> seriesList, Hec.Dss.TimeSeries ts)
    {
      return seriesList.Count > 0 && seriesList[0].StartDateTime != ts.StartDateTime;
    }

    /// <summary>
    /// C:000002|T:0212019
    /// </summary>
    /// <param name="Fpart"></param>
    /// <returns></returns>
    private static void ParseFPart(string Fpart, out int memberidx, out DateTime issueDate)
    {
      memberidx = int.Parse(Fpart.Split('|')[0].Split(':').Last().TrimStart('0'));
      int idx = Fpart.IndexOf("T:");
      if (idx < 0)
        throw new Exception("Could not parse issue date from '" + Fpart + "'");

      Fpart = Fpart.Substring(idx + 2);

      int year = Convert.ToInt32(Fpart.Substring(3));
      string sday = Fpart.Substring(0, 3);
      int day = Convert.ToInt32(sday);
      issueDate = new DateTime(year, 1, 1).AddDays(day - 1).AddHours(12);
    }


    public static Watershed ReadTimeSeriesProfiles(string watershedName, DateTime start, DateTime end, string dssFileName)
    {
      Watershed rval = new Watershed(watershedName);
      float[,] profile = null;

      using (DssReader dss = new DssReader(dssFileName))
      {
        Console.WriteLine("Reading" + dssFileName);
        DssPathCollection dssPaths = dss.GetCatalog(); // sorted
                                                       // var dssPaths = rawDssPaths.OrderBy(a => a, new PathComparer()).ToArray(); // sorted
        int size = dssPaths.Count();
        if (size == 0)
        {
          throw new Exception("Empty DSS catalog");
        }
        // /RUSSIANNAPA/APCC1/FLOW/01SEP2019/1HOUR/|T:0212019/
        for (int i = 0; i < size; i++)
        {
          if (i % 100 == 0)
            Console.Write(".");

          DssPath path = dssPaths[i];
          DateTime issueDate = ParseIssueDate(path.Fpart);

          if (issueDate >= start && issueDate <= end
            && path.Apart == watershedName)
          {
            var ts = dss.GetTimeSeriesProfile(path);
            ArrayUtility.TransposeDoubleToFloat(ts.Values, ref profile);
            rval.AddForecast(path.Bpart, issueDate, profile, ts.Times);
          }
        }
      }
      return rval;
    }

    /// <summary>
    /// parse issue date from part F:
    /// C:000002|T:0212019
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static DateTime ParseIssueDate(string input)
    {
      int idx = input.IndexOf("T:");
      if (idx < 0)
        throw new Exception("Could not parse issue date from '" + input + "'");

      input = input.Substring(idx + 2);

      int year = Convert.ToInt32(input.Substring(3));
      string sday = input.Substring(0, 3);
      int day = Convert.ToInt32(sday);
      DateTime issueDate = new DateTime(year, 1, 1).AddDays(day - 1).AddHours(12);
      return issueDate;
    }
        
  }
}

