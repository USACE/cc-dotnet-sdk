using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usace.cc.plugin
{
  public class Status
  {
    public enum StatusLevel
    {
      COMPUTING, //Status = "Computing"
      FAILED,    //Status = "Failed"
      SUCCEEDED, //Status = "Succeeded"
    }
    public Status(StatusLevel level, int progress)
    {
      Progress = progress;
      GetStatus = level;
    }
    public int Progress { get; set; }
    public StatusLevel GetStatus { get; set; }
  }

}
