namespace usace.cc.plugin
{
  public class Message
    {
        public string message { get; private set; }
        public Message(string msg="")
        {
          message = msg;
        }
    }
}
