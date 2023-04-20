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
    internal int Progress { get; set; }
    internal StatusLevel GetStatus { get; set; }
  }

}
