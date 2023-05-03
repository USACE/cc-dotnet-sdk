namespace Usace.CC.Plugin
{
  public class GetObjectInput
  {
    private string fileName;
    private string fileExtension;
    private StoreType sourceStoreType;
    private string sourceRootPath;

    public GetObjectInput(string fileName, StoreType sourceStoreType, string sourceRootPath, string fileExtension)
    {
      this.fileName = fileName;
      this.sourceStoreType = sourceStoreType;
      this.sourceRootPath = sourceRootPath;
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

 
  }

}