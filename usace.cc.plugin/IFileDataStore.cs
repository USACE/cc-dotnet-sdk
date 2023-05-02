namespace Usace.CC.Plugin
{
  public interface IFileDataStore
  {
    public Boolean Copy(IFileDataStore destStore, String srcPath, String destPath);
    public Stream Get(String path);
    public Boolean Put(Stream data, String path);
    public Boolean Delete(String path);
  }
}
