namespace usace.cc.plugin
{
  public class PullObjectInput
  {
    private string fileName;
    private string fileExtension;
    private StoreType sourceStoreType;
    private string sourceRootPath;
    private string destRootPath;
    public PullObjectInput(string fileName, StoreType sourceStoreType, string sourceRootPath,
                        string destRootPath, string fileExtension)
    {
      this.fileName = fileName;
      this.sourceStoreType = sourceStoreType;
      this.sourceRootPath = sourceRootPath;
      this.destRootPath = destRootPath;
      this.fileExtension = fileExtension;
    }
    public string FileName
    {
      get { return fileName; }
    }

    public string FileExtension
    {
      get { return fileExtension; }
    }

    public StoreType SourceStoreType
    {
      get { return sourceStoreType; }
    }

    public string SourceRootPath
    {
      get { return sourceRootPath; }
    }

    public string DestRootPath
    {
      get { return destRootPath; }
    }


  }
}
