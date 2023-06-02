namespace Usace.CC.Plugin
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
