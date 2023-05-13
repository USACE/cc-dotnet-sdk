using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usace.CaliforniaNevadaRfc
{
  [DebuggerDisplay("{Name}")]
  public class Location
  {
    public Location(string name, Watershed watershed)
    {
      this.Name = name;
      Forecasts = new List<Forecast>();
      this.Watershed = watershed;
    }
    public string Name { get; set; }
    /// <summary>
    /// Parent Watershed
    /// </summary>
    public Watershed Watershed { get; set; } 
    
    /// <summary>
    /// List of forecasts 
    /// </summary>
    public List<Forecast> Forecasts { get; set; }

    internal Forecast AddForecast(DateTime issueDate, float[,] ensemble,DateTime[] timeStamps)
    {
      Forecast f = new Forecast(this, issueDate,ensemble,timeStamps);
      Forecasts.Add(f);
      return f;
    }
  }
}
