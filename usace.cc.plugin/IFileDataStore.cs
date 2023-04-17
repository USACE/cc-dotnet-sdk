using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usace.cc.plugin
{
  public interface IFileDataStore
  {
    public Boolean Copy(IFileDataStore destStore, String srcPath, String destPath);
    public Stream Get(String path);
    public Boolean Put(Stream data, String path);
    public Boolean Delete(String path);
  }
}
