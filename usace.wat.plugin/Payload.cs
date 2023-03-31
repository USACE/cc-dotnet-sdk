namespace usace.cc.plugin
{
    public class Payload
    {
    private Dictionary<string, Object> Attributes { get; set; } 
    private DataStore[] Stores { get; set; }
    private DataSource[] Inputs { get; set; }
    private DataSource[] Outputs { get; set; }
  }
}
