using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using usace.cc.plugin;

namespace usace.cc.plugin.test
{
  public class PluginManagerTest
  {

    [Fact]
    public void InputOutputSubstutions()
    {
      Environment.SetEnvironmentVariable("CC_EVENT_NUMBER", "123", EnvironmentVariableTarget.Process);
      string s1 = "/runs/{ENV::CC_EVENT_NUMBER}/seedgenerator/seeds.json";
      var attributes = new Dictionary<string, string>();
      var s2 = usace.cc.plugin.Utility.PathSubstitution(s1, attributes);
      Assert.Equal("/runs/123/seedgenerator/seeds.json", s2);// requires some setup

    }

    
  }
}
