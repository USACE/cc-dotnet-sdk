using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usace.cc.plugin
{
  /// <summary>
  /// Repository of DataSources
  /// 
  /// example:  S3 bucket
  /// </summary>
  public class DataStore
  {
    public String Name { get; set; }
    public String ID { get; set; }
    public Dictionary<String, String> Parameters { get; set; }
    public StoreType StoreType { get; set; }
    public String DsProfile { get; set; }
    public Object Session { get; set; }
  }
}
