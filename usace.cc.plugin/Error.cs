using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usace.cc.plugin
{
  public class Error
  {
    public enum Level
    {
      DEBUG,
      INFO,
      WARN,
      ERROR,
      FATAL,
      PANIC,
      DISABLED,
    }
    public string? ErrorMessage { get; set; }
    public Level ErrorLevel { get; set; }
  }
}
