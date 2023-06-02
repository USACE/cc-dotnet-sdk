namespace Usace.CC.Plugin
{
  public class Error
  {
    public enum Level
    {
      DEBUG,
      INFO,
      WARN,
      ERROR,
      FATAL,
      PANIC,
      DISABLED,
    }
    public string ErrorMessage { get; set; }
    public Level ErrorLevel { get; set; }
  }
}
