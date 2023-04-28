namespace usace.cc.plugin
{
  public class PutObjectInput
  {
    private string fileName;
    private string fileExtension;
    private StoreType destStoreType;
    private ObjectState objectState;
    private byte[] data;
    private string sourcePath;
    private string destPath;

    public string FileName
    {
      get { return fileName; }
    }

    public string FileExtension
    {
      get { return fileExtension; }
    }

    public StoreType DestinationStoreType
    {
      get { return destStoreType; }
    }

    public ObjectState ObjectState
    {
      get { return objectState; }
    }

    public byte[] Data
    {
      get { return data; }
    }

    public string SourcePath
    {
      get { return sourcePath; }
    }

    public string DestinationPath
    {
      get { return destPath; }
    }

    public PutObjectInput(string fileName, StoreType destStoreType, string sourcePath, string destPath, string fileExtension, ObjectState state, byte[] data)
    {
      this.fileName = fileName;
      this.destStoreType = destStoreType;
      this.sourcePath = sourcePath;
      this.destPath = destPath;
      this.fileExtension = fileExtension;
      this.data = data;
      this.objectState = state;
    }
  }
}
