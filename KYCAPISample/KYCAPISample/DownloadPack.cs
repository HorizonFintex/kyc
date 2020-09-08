using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace KYCAPISample
{
  public class DownloadPack
  {
    public PackItem Pack { get; set; }
  }
  public class PackItem
  {
    public string Data { get; set; }
  }
}
