﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace usace.cc.plugin.test
{
  public class BucketTesting
  {
    private static Payload CreateTestPayload(string storeName)
    {
      var payload = new Payload();
      var id = "6ba7b810-9dad-11d1-80b4-00c04fd430c8";

      var ds = new DataStore()
      {
        ID = id,
        Name = storeName,
        StoreType = StoreType.S3,
        DsProfile = "FFRD",
        Parameters = new Dictionary<string, string>(),
        Session = null
      };
      ds.Parameters.Add("root", "/muncie");
      payload.Stores = new DataStore[] { ds };
      var inputs = new List<DataSource>();
      inputs.Add(new DataSource() {
        Name = "seedgenerator",
        ID = id,
        Paths = new string[] { "seeds/sg.json" },
        StoreName = storeName
      });

      payload.Inputs = inputs.ToArray();

      var output = new List<DataSource>();
      output.Add(new DataSource()
      {
        Name = "seedgenerator",
        ID = id,
        Paths = new string[] { "seeds/sg.json" },
        StoreName = storeName
      });

      payload.Outputs = output.ToArray();
      return payload;
    }

    [Fact]
    public void Test1()
    {
      var payload = CreateTestPayload("FFRD");
      FileDataStoreS3 s3 = new FileDataStoreS3(payload.Stores[0]);


    }
  }
}
