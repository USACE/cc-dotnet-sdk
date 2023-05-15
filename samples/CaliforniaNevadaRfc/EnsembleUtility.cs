using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usace.CaliforniaNevadaRfc
{
  public class EnsembleUtility
  {
    /// <summary>
    /// Reads ensemble for single location and single date
    /// </summary>
    /// <param name="watershedName"></param>
    /// <param name="location"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Watershed ReadEnsembleForecast(string watershedName, string location, DateTime t)
    {
      var tmp = Path.GetTempPath();
      var csvFileName = RfcDownloader.DownloadToCsv(watershedName, t, tmp);
      var r = new CsvEnsembleReader(tmp);
      var rval = r.Read(watershedName, t, t,location);
      return rval;
    }
  }
}
