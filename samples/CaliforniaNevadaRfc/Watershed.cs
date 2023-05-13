using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usace.CaliforniaNevadaRfc
{
  [DebuggerDisplay("{Name}")]
  public class Watershed
  {

    public Watershed(string name)
    {
      this.Name = name;
      Locations = new List<Location>(10);
    }
    public string Name { get; set; }

    public List<Location> Locations { get; set; }

    public Forecast AddForecast(string locName, DateTime issueDate, float[,] ensemble, DateTime[] timeStamps)
    {
      Location loc = GetOrCreateLocation(locName);

      var rval = loc.AddForecast(issueDate, ensemble, timeStamps);
      return rval;
    }

    private Location GetOrCreateLocation(string locName)
    {
      int idx = Locations.FindIndex(x => x.Name.Equals(locName));
      Location loc = null;
      if (idx >= 0)
        loc = Locations[idx];
      else
      {
        loc = new Location(locName, this);
        Locations.Add(loc);
      }

      return loc;
    }

    public Watershed CloneSubset(int takeCount)
    {
      int count = 0;
      var retn = new Watershed(this.Name);
      foreach (Location loc in this.Locations)
      {
        foreach (Forecast f in loc.Forecasts)
        {
          retn.AddForecast(f.Location.Name, f.IssueDate, f.Ensemble,f.TimeStamps);
          count++;
          if( count >= takeCount)
            break;
        }
      }
      return retn;

    }

  }
}
