namespace usace.cc.plugin
{
    public class Payload
    {
    internal Dictionary<string, Object> Attributes { get; set; } 
    internal DataStore[] Stores { get; set; }
    internal DataSource[] Inputs { get; set; }
    internal DataSource[] Outputs { get; set; }

  }
}
