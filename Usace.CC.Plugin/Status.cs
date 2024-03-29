﻿namespace Usace.CC.Plugin
{
  public class Status
  {
    public enum StatusLevel
    {
      COMPUTING, //Status = "Computing"
      FAILED,    //Status = "Failed"
      SUCCEEDED, //Status = "Succeeded"
    }
    public Status(StatusLevel level, int progress)
    {
      Progress = progress;
      GetStatus = level;
    }
    public int Progress { get; set; }
    public StatusLevel GetStatus { get; set; }
  }

}
