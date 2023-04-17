using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usace.cc.plugin
{
  internal class Utility
  {
    internal static string GetEnv(string name)
    {
      string? x = Environment.GetEnvironmentVariable(name);

      if (x == null)
        return "";
      else
        return x;

    }
  }
}
