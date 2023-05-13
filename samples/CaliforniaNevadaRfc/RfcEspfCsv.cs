using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hec.TimeSeries.Ensemble
{
  /// <summary>
  /// Supports parsing a RFC file that has a single location, and multiple ensemble members
  /// example: https://www.nwrfc.noaa.gov/chpsesp/ensemble/watersupply/HCDI1W_SQIN.ESPF10.csv
  /// </summary>
  public class RfcEspfCsv
   {
      public DateTime IssueDate;
      private double[,] Data;
      public string[] ColumnNames;
      public DateTime[] TimeStamps { get; private set; }
      public string Configuration { get; set; }
      public string Units { get; set; }
      public string RfcFile { get; set; }

      public double[] this[int index] {
         get {
            var rval = new double[Data.GetLength(0)];
            for (int i = 0; i < rval.Length; i++)
            {
               rval[i] = Data[i,index];
            }
            return rval;
         }
      }

      string[] rows;
      /// <summary>
      /// CSV file format ESPF 
      /// example
      /// https://www.nwrfc.noaa.gov/chpsesp/ensemble/watersupply/HCDI1W_SQIN.ESPF10.csv
      /// 
      /// first rows have meta-data
      /// FILE:HCDI1W_SQIN.ESPF10
      /// ISSUED:2021-12-08_19:56_GMT
      /// CONFIGURATION:WATER_SUPPLY
      /// YEARS:WYR
      /// UNITS:KCFS
      /// QPFDAYS:10,
      /// 
      /// First column is date/time
      /// </summary>
      /// <param name="fileName"></param>
      public RfcEspfCsv(string fileName)
      {
         rows = File.ReadAllLines(fileName);
         IssueDate = ReadDateTime("ISSUED");
         Configuration = LookupLabel("CONFIGURATION");
         Units = LookupLabel("UNITS");
         RfcFile = LookupLabel("FILE");

         ParseData();

      }


      /// <summary>
      /// Parse data swaping axis
      /// rows represent timesteps
      /// columns represent locations
      /// </summary>
      /// <param name="rows"></param>
      private void ParseData()
      {
         int idx2 = FindLastRowIndex(rows);
         int idxYears = IndexStartingWith("FCST_VALID_TIME_GMT");
         ColumnNames = rows[idxYears].Split(',');
         int idx1 = idxYears + 1;
         int rowCount = idx2 - idx1 + 1;
         int columnCount = ColumnNames.Length - 1; // date column will not be part of data
         TimeStamps = new DateTime[rowCount];
         Data = new double[rowCount, columnCount]; 
         for (int rowIdx = 0; rowIdx < rowCount; rowIdx++)
         {
            string[] values = rows[rowIdx + idx1].Split(',');
            TimeStamps[rowIdx] = ParseDateTime(values[0]); // first column is DateTime
            for (int columnIdx = 0; columnIdx < columnCount; columnIdx++)
            {
               var d = double.Parse(values[columnIdx + 1]);
               Data[rowIdx,columnIdx] = d;
            }
         }
      }


      /// <summary>
      /// find last row of data.
      /// some files have empty lines at the bottom.
      /// </summary>
      /// <param name="rows"></param>
      /// <returns></returns>
      private int FindLastRowIndex(string[] rows)
      {
         for (int i = rows.Length - 1; i > 0; i--)
         {
            if (rows[i].Trim() != "")
               return i;
         }
         return -1;
      }


   private DateTime ReadDateTime(string label)
      {
         string s = LookupLabel(label);
         //ISSUED:2021-12-08_19:56_GMT
         if (s != "")
         {
            s= s.Replace("_", " ");
            s = s.Replace("GMT","").Trim();

            return DateTime.Parse(s);
         }
         return default(DateTime);
      }

      private int IndexStartingWith(string value)
      {
         for (int i = 0; i < rows.Length; i++)
         {
            var s = rows[i];
            if (s.StartsWith(value))
               return i;
               
         }
         return -1;

      }
      private string LookupLabel(string label)
      {
         int idx = IndexStartingWith(label+":");
         if(idx >=0)
            return rows[idx].Substring(label.Length + 1);

         return "";
      }

     static DateTime ParseDateTime(string dt)
      {//2021-12-08_19:56_GMT
         return DateTime.Parse(dt);
      string[] dateTime = dt.Split(' ');
      string[] yyyymmdd = dateTime[0].Split('-');
      string[] hhmmss = dateTime[1].Split(':');
      DateTime output = new DateTime(int.Parse(yyyymmdd[0]), int.Parse(yyyymmdd[1]), int.Parse(yyyymmdd[2]), int.Parse(hhmmss[0]), int.Parse(hhmmss[1]), int.Parse(hhmmss[2]));
      return output;
    }
  }
}
