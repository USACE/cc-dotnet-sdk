using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usace.cc.plugin
{
  /// <summary>
  /// CCStore is a special Store used by plugins to pull payloads.
  /// </summary>
  public interface CcStore
  {
    public bool PutObject(PutObjectInput input);
    public bool PullObject(PullObjectInput input);
    public byte[] GetObject(GetObjectInput input);
    public Payload GetPayload();
    //public void SetPayload(Payload payload); only used in the go sdk to support cloudcompute which is written in go.
    public String RootPath();
    public bool HandlesDataStoreType(StoreType datastoretype);
  }
}
