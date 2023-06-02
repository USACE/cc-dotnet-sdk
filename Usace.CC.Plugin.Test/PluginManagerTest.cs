using System;
using System.Collections.Generic;
using Xunit;

namespace Usace.CC.Plugin.Test
{
  public class PluginManagerTest
  {

    [Fact]
    public void InputOutputSubstutions()
    {
      Environment.SetEnvironmentVariable("CC_EVENT_NUMBER", "123", EnvironmentVariableTarget.Process);
      string s1 = "/runs/{ENV::CC_EVENT_NUMBER}/seedgenerator/seeds.json";
      var attributes = new Dictionary<string, string>();
      var s2 = Utility.PathSubstitution(s1, attributes);
      Assert.Equal("/runs/123/seedgenerator/seeds.json", s2);// requires some setup

    }

    
  }
}
