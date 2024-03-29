﻿namespace Usace.CC.Plugin
{
  /// <summary>
  /// CCStore is a special Store used by plugins to pull payloads.
  /// </summary>
  public interface CcStore
  {
    public Task<bool> PutObject(PutObjectInput input);
    public Task<bool> PullObject(PullObjectInput input);
    public Task<byte[]> GetObject(GetObjectInput input);
    public Task<Payload> GetPayload();
    //public void SetPayload(Payload payload); only used in the go sdk to support cloudcompute which is written in go.
    /// <summary>
    /// For S3 RootPath() returns the bucket Name.
    /// </summary>
    /// <returns></returns>
    public String RootPath();
    public bool HandlesDataStoreType(StoreType datastoretype);
  }
}
