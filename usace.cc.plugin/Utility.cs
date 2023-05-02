using System.Text.RegularExpressions;

namespace usace.cc.plugin
{
  public class Utility
  {
    internal static string GetEnv(string name)
    {
      string x = Environment.GetEnvironmentVariable(name);

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
    public static string PathSubstitution(string path, Dictionary<string, string> attributes)
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
          if( attributes.TryGetValue(variableName, out string str))
          { 
            if( str != null )
               rval = rval.Replace(m.Value, str.ToString());
          }
        }
      }

      return rval;
    }

    //https://stackoverflow.com/questions/34144494/how-can-i-get-the-bytes-of-a-getobjectresponse-from-s3
    internal static byte[] ReadBytes(Stream stream)
    {
      byte[] buffer = new byte[256];
      using (MemoryStream ms = new MemoryStream())
      {
        int read;
        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
          ms.Write(buffer, 0, read);
        }
        return ms.ToArray();
      }
    }

  }
}
