using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace usace.cc.plugin
{
  /// <summary>
  /// PluginManager is intended to be used by plugin developers.
  /// </summary>
  public class PluginManager
  {
    Logger _logger;
    CcStoreS3 cs;
    Payload _payload;
    Regex regex;
    public PluginManager()
    {
      string sender = Utility.GetEnv(EnvironmentVariables.CC_PLUGIN_DEFINITION);
      _logger = new Logger(sender, Error.Level.WARN);
      cs = new CcStoreS3();
      try
      {
        _payload = cs.GetPayload();
        int i = 0;
        foreach (DataStore store in _payload.Stores)
        {
          switch (store.StoreType)
          {
            case StoreType.S3:
              store.Session = new FileDataStoreS3(store);
              _payload.Stores[i] = store;
              break;
            case StoreType.WS:
            case StoreType.RDBMS:
              Console.WriteLine("WS and RDBMS session instantiation is the responsibility of the plugin.");
              break;
            default:
              Console.WriteLine("Invalid Store type");//what type was provided?
              break;
          }
          i++;
        }
        substitutePathVariables(_payload.Inputs);
        substitutePathVariables(_payload.Outputs);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        Console.WriteLine(e.StackTrace);
      }

    }
    private void substitutePathVariables(DataSource[] sources)
    {
      for (int i = 0; i < sources.Length; i++)
      {
        for (int j = 0; j < sources[i].Paths.Length; j++)
        {
          sources[i].Paths[j] = Utility.PathSubstitution(sources[i].Paths[j],_payload);
        }
      }
    }

   
    public FileDataStore getFileStore(String storeName)
    {
      return (FileDataStore)findDataStore(storeName).getSession();//check for nil?
    }
    public DataStore getStore(String storeName)
    {
      return findDataStore(storeName);
    }
    public DataSource getInputDataSource(String name)
    {
      return findDataSource(name, getInputDataSources());
    }
    public DataSource getOutputDataSource(String name)
    {
      return findDataSource(name, getOutputDataSources());
    }
    public DataSource[] getInputDataSources()
    {
      return _payload.getInputs();
    }
    public DataSource[] getOutputDataSources()
    {
      return _payload.getOutputs();
    }
    public byte[] getFile(DataSource ds, int path)
    {
      FileDataStore store = getFileStore(ds.getStoreName());
      InputStream reader = store.Get(ds.getPaths()[path]);
      byte[] data;
      try
      {
        data = reader.readAllBytes();
        return data;
      }
      catch (IOException e)
      {
        e.printStackTrace();
        return null;
      }
    }
    public boolean putFile(byte[] data, DataSource ds, int path)
    {
      FileDataStore store = getFileStore(ds.getStoreName());
      return store.Put(new ByteArrayInputStream(data), ds.getPaths()[path]);
    }
    public boolean fileWriter(InputStream inputstream, DataSource destDs, int destPath)
    {
      FileDataStore store = getFileStore(destDs.getStoreName());
      return store.Put(inputstream, destDs.getPaths()[destPath]);
    }
    public InputStream fileReader(DataSource ds, int path)
    {
      FileDataStore store = getFileStore(ds.getStoreName());
      return store.Get(ds.getPaths()[path]);
    }
    public InputStream fileReaderByName(String dataSourceName, int path)
    {
      DataSource ds = findDataSource(dataSourceName, getInputDataSources());
      return fileReader(ds, path);
    }
    public void setLogLevel(ErrorLevel level)
    {
      _logger.setErrorLevel(level);
    }
    public void LogMessage(Message message)
    {
      _logger.LogMessage(message);
    }
    public void LogError(Error error)
    {
      _logger.LogError(error);
    }
    public void ReportProgress(Status report)
    {
      _logger.ReportStatus(report);
    }
    public int EventNumber()
    {
      //Object result = _payload.getAttributes().get(EnvironmentVariables.CC_EVENT_NUMBER);
      String val = System.getenv(EnvironmentVariables.CC_EVENT_NUMBER);
      int eventNumber = Integer.parseInt(val);
      return eventNumber;
    }
    private DataSource findDataSource(String name, DataSource[] dataSources)
    {
      for (DataSource dataSource : dataSources)
      {
        if (dataSource.getName().equalsIgnoreCase(name))
        {
          return dataSource;
        }
      }
      return null;
    }
    private DataStore findDataStore(String name)
    {
      for (DataStore dataStore : _payload.getStores())
      {
        if (dataStore.getName().equalsIgnoreCase(name))
        {
          return dataStore;
        }
      }
      return null;
    }
  }
}
