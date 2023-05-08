namespace Usace.CC.Plugin
{
  public interface IFileDataStore
  {
    public Task<bool> Copy(IFileDataStore destStore, String srcPath, String destPath);
    public Task<Stream> Get(String path);
    public Task<bool> Put(Stream data, String path);
    public Task<bool> Delete(String path);
  }
}
