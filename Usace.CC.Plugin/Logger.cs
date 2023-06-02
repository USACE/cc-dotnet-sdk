using System.Runtime.CompilerServices;

namespace Usace.CC.Plugin
{
  internal class Logger
  {
    public Logger(string sender, Error.Level level)
    {
      Sender = sender;
      Level = level;
    }
    public Logger()
    {
      Sender = "";
    }
    public Error.Level Level { private get; set; }
    public string Sender { get; set; }

    private string TimeStamp()
    {
      return DateTime.Now.ToString();
    }
    public void LogMessage(Message message)
    {
      String line = Sender + ":" + TimeStamp() + "\n\t" + message.message + "\n";
      Write(line);
    }

    public void ReportStatus(Status report)
    {
      String line = Sender + ":" + report.GetStatus + ":" +TimeStamp() + "\n\t" + report.Progress + " percent complete." + "\n";
      Write(line);
    }

    public void LogError(Error error, [CallerMemberName] string memberName = "",
          [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNo = 0)
    {
      string s = "";
      string message = error.ErrorMessage;
      if (Sender == "")
      {
        s = "Unknown Sender";
      }
      else
      {
        s = Sender;
      }
      if (Level.CompareTo(Error.Level.DEBUG) == 0)
      {
        Write(s + " issues " + Level.ToString() + " at " + TimeStamp() + " from file " + filePath + " on line " + lineNo + " in method name " + memberName + "\n\t" + message + "\n");
      }
      else
        if (Level.CompareTo(Error.Level.ERROR) >= 0)
      {
        Write(s + " issues " + Level.ToString() + " at " + TimeStamp() + "\n\t" + message + "\n");
      }

    }
    private void Write(string message)
    {
      Console.WriteLine(message);
    }
  }
}