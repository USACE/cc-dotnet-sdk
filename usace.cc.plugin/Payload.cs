namespace usace.cc.plugin
{
    public class Payload
    {
    public Dictionary<string, Object> Attributes { get; set; }
    public DataStore[] Stores { get; set; }
    public DataSource[] Inputs { get; set; }
    public DataSource[] Outputs { get; set; }

  }
}
