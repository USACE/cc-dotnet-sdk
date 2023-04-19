namespace usace.cc.plugin
{
    public class Payload
    {
    internal Dictionary<string, Object> Attributes { get; set; } 
    internal DataStore[] Stores { get; set; }
    internal DataSource[] Inputs { get; set; }
    internal DataSource[] Outputs { get; set; }

    public Payload()
    {
      Attributes = new Dictionary<string, Object>();
      Stores = new DataStore[0];
      Inputs = new DataSource[0];
      Outputs = new DataSource[0];
    }
  }
}
