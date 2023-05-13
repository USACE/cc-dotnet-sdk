using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usace.CaliforniaNevadaRfc
{
  [DebuggerDisplay("Forecast [{Ensemble.GetLength(0)},{Ensemble.GetLength(1)}]")]
  public class Forecast
  {
    public Forecast(Location location, DateTime issueDate, float[,] ensemble,DateTime[] timeStamps)
    {
      this.Location = location;
      this.IssueDate = issueDate;
      this.Ensemble = ensemble;
      this.TimeStamps = timeStamps;
    }

    /// <summary>
    /// Location of this forecast
    /// </summary>
    public Location Location { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime[] TimeStamps { get; set; }

    public float[,] Ensemble { get; set; }

   

    public float[] EnsembleMember(int index)
    {
      float[] rval = null;
      EnsembleMember(index, ref rval);
      return rval;
    }

    public void EnsembleMember(int index, ref float[] data)
    {
      int arrWidth = Ensemble.GetLength(1);
      if (data == null || data.Length != arrWidth)
        data = new float[arrWidth];

      // This block-copies a single row, equivalent to the for loop below
      Buffer.BlockCopy(Ensemble, index * arrWidth * sizeof(float), data, 0, 
        arrWidth * sizeof(float));

      //for (int i = 0; i < rval.Length; i++)
      //{
      //  data[i] = Ensemble[index, i];
      //}
    }



  }
}
