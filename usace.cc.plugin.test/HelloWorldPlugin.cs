using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usace.CC.Plugin.Test
{
  internal class HelloWorldPlugin
  {
    public static async Task Main(string[] args)
    {
      Console.WriteLine("hello world");
      var pm = await PluginManager.CreateAsync();
      pm.LogMessage(new Message("hello world, from .net-sdk-PluginManager."));
      var eventNumber = pm.EventNumber();
      pm.LogMessage(new Message("event number is: " + eventNumber));
      var payload = pm.Payload;
      //var obj = pm.getFile(payload.Inputs[0],0);
      if( payload.Inputs.Length != 1)
      {
        pm.LogMessage(new Message("expected one input. Found " + payload.Inputs.Length+" inputs"));
        pm.ReportProgress(new Status(Status.StatusLevel.FAILED,0));
        return;
      }  
    }
  }
}
