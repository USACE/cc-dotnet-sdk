namespace Usace.CC.Plugin
{
  /// <summary>
  /// Generic data source -- like a traditional file
  /// used for Input and Outputs in a payload
  /// 
  /// examples: csv file, database, shapefile, S3 object
  /// </summary>
  public class DataSource
  {
    public string Name { get; set; }
    public string ID { get; set; }
    public string StoreName { get; set; }
    public string[] Paths { get; set; } // specific key or set of keys,  shape_file.[shp|shx|dbf]
  }
}
