namespace usace.cc.plugin
{
    public class Payload
    {
    public Dictionary<string, Object> Attributes { get; set; }
    public DataStore[] Stores { get; set; }
    public DataSource[] Inputs { get; set; }
    public DataSource[] Outputs { get; set; }

    public Payload()
    {
      Attributes = new Dictionary<string, Object>();
      Stores = new DataStore[0];
      Inputs = new DataSource[0];
      Outputs = new DataSource[0];
    }
  }
}
