namespace Usace.CC.Plugin
{
  /// <summary>
  /// PluginManager is intended to be used by plugin developers.
  /// </summary>
  public class PluginManager
  {
    Logger _logger;
    CcStoreS3 cs;
    public Payload Payload { get; private set; }

    private PluginManager()  {    }

    public static Task<PluginManager> CreateAsync()
    {
      var rval = new PluginManager();
      return rval.InitializeAsync();
    }

    private async Task<PluginManager> InitializeAsync()
    {
      string sender = Utility.GetEnv(EnvironmentVariables.CC_PLUGIN_DEFINITION);
      _logger = new Logger(sender, Error.Level.WARN);
      cs = new CcStoreS3();
      try
      {
        Payload = await cs.GetPayload();
        int i = 0;
        foreach (DataStore store in Payload.Stores)
        {
          switch (store.StoreType)
          {
            case StoreType.S3:
              store.Session = new FileDataStoreS3(store);
              Payload.Stores[i] = store;
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
        substitutePathVariables(Payload.Inputs);
        substitutePathVariables(Payload.Outputs);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        Console.WriteLine(e.StackTrace);
      }
      return this;
    }
    private void substitutePathVariables(DataSource[] sources)
    {
      for (int i = 0; i < sources.Length; i++)
      {
        for (int j = 0; j < sources[i].Paths.Length; j++)
        {
          sources[i].Paths[j] = Utility.PathSubstitution(sources[i].Paths[j],Payload.Attributes);
        }
      }
    }

    public IFileDataStore getFileStore(String storeName)
    {
      return (IFileDataStore)findDataStore(storeName);
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
      return Payload.Inputs;
    }
    public DataSource[] getOutputDataSources()
    {
      return Payload.Outputs;
    }
    public async Task<byte[]> getFile(DataSource ds, int path)
    {
      IFileDataStore store = getFileStore(ds.StoreName);
      var reader = await store.Get(ds.Paths[path]);
      byte[] data = new byte[0];
      try
      {
        data = Utility.ReadBytes(reader);
        return data;
      }
      catch (IOException e)
      {
        Console.WriteLine(e.Message);
        return null;
      }
    }
    public async Task<bool> PutFile(byte[] data, DataSource ds, int path)
    {
      var store = getFileStore(ds.StoreName);
      var stream = new MemoryStream(data);
      return await store.Put(stream, ds.Paths[path]);
    }
    public Task<bool> FileWriter(Stream inputstream, DataSource destDs, int destPath)
    {
      var store = getFileStore(destDs.StoreName);
      return store.Put(inputstream, destDs.Paths[destPath]);
    }
    public async Task<Stream> FileReader(DataSource ds, int path)
    {
      var store = getFileStore(ds.StoreName);
      return await store.Get(ds.Paths[path]);
    }
    public Task<Stream> FileReaderByName(String dataSourceName, int path)
    {
      DataSource ds = findDataSource(dataSourceName, getInputDataSources());
      return FileReader(ds, path);
    }
    public void SetLogLevel(Error.Level level)
    {
      _logger.Level = level;
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
      string val = Utility.GetEnv(EnvironmentVariables.CC_EVENT_NUMBER);
      if(int.TryParse(val,out int eventNumber))
      {
        return eventNumber;
      }
      return -1;
    }
    private DataSource findDataSource(String name, DataSource[] dataSources)
    {
      foreach (DataSource dataSource in dataSources)
      {
        if (dataSource.Name.Equals(name,StringComparison.OrdinalIgnoreCase ) )
        {
          return dataSource;
        }
      }
      return null;
    }
    private DataStore findDataStore(String name)
    {

      foreach (DataStore dataStore in Payload.Stores)
      {
        if (name.Equals(dataStore.Name, StringComparison.OrdinalIgnoreCase))
        {
          return dataStore;
        }
      }
      return null;
    }
  }
}
