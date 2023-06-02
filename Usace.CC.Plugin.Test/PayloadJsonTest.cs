using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Usace.CC.Plugin.Test
{
  public class PayloadJsonTest
  {
		static string json = @"{
	""stores"": [{
		""Name"": ""FFRD"",
		""ID"": ""6ba7b810-9dad-11d1-80b4-00c04fd430c8"",
		""StoreType"": ""S3"",
		""DsProfile"": ""FFRD"",
		""Parameters"": {
			""root"": ""/muncie""
		},
		""Session"": null
	}],
	""inputs"": [{
		""Name"": ""seedgenerator"",
		""ID"": ""6ba7b810-9dad-11d1-80b4-00c04fd430c8"",
		""Paths"": [""seeds/sg.json""],
		""StoreName"": ""FFRD""
	}],
	""outputs"": [{
		""Name"": ""seeds"",
		""ID"": ""6ba7b810-9dad-11d1-80b4-00c04fd430c8"",
		""Paths"": [""/runs/{ENV::CC_EVENT_NUMBER}/seedgenerator/seeds.json""],
		""StoreName"": ""FFRD""
	}]
}";


		[Fact]
    public void FromJson()
    {
			 var p = Payload.FromJson(json);
			  Assert.Equal("seedgenerator", p.Inputs[0].Name);
			  Assert.Equal("FFRD", p.Inputs[0].StoreName);
			  System.Console.WriteLine(p);
		}



}
}
