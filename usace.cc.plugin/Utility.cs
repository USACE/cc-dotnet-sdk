using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace usace.cc.plugin
{
  public class Utility
  {
    internal static string GetEnv(string name)
    {
      string? x = Environment.GetEnvironmentVariable(name);

      if (x == null)
        return "";
      else
        return x;
    }

    /// <summary>
    /// Replaces placeholders with values from environment variables, or Attributes in the payload
    ///  
    /// </summary>
    /// <param name="path"></param>
    /// <param name="payload"></param>
    /// <returns></returns>
    public static string PathSubstitution(string path, Payload payload)
    {

      Regex regex = new Regex("{(?<type>ENV|ATTR)::(?<var>[^{}]+)}");
      var matches = regex.Matches(path);
      
      string rval = path;

      foreach (Match m in matches)
      {
        string variableName = m.Groups["var"].Value;
        string type = m.Groups["type"].Value;

        if (type == "ENV")
        {
          var envVar = Environment.GetEnvironmentVariable(variableName);
          if ( !String.IsNullOrEmpty(envVar))
          {
            rval = rval.Replace(m.Value, envVar);
          }
        }
        else if (type == "ATTR")
        {
          object? obj;
          if (payload.Attributes.TryGetValue(variableName, out obj))
          { 
            if( obj != null )
               rval = rval.Replace(m.Value, obj.ToString());
          }
        }
      }

      return rval;
    }

  }
}
