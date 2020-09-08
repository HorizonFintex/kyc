using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Security.Cryptography;
using System.IO.Compression;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KYCAPISample
{
  public class KYCUser
  {
    private HttpClient _client;
    private readonly string txtFile = "test.txt";
    private readonly string zipFile = "test.zip";
    private readonly string tempPath = "temp";
    public KYCUser()
    {
      _client = new HttpClient();
    }

    private string ComputeHash(string kycPack)
    {

      var decodedKyc = Convert.FromBase64String(kycPack);
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < 10; i++)
      {
        sb.Append(decodedKyc[i] + ", ");
      }

      for (int i = decodedKyc.Length - 10; i < decodedKyc.Length; i++)
      {
        sb.Append(decodedKyc[i] + ", ");
      }

      sb.Append(" ... ");
      SHA256 mySHA256 = SHA256Managed.Create();
      var localhash = mySHA256.ComputeHash(decodedKyc);
      var localhashString = BitConverter.ToString(localhash).Replace("-", string.Empty);

      return localhashString;
    }

    private HttpContent GenerateHttpContent(string kycPack)
    {
      var hash = ComputeHash(kycPack);
      var requestBody = $@"{{ ""kyc"": ""{kycPack}"",""serviceprovider"": ""{Globals.kycUploadServiceprovider}"",""firstname"": ""James R"",""hash"": ""{hash}"",""lastname"": ""Polk"",""email"": ""{Globals.kycUploadEmail}"",""nationality"": ""US Citizen"",""contribution"": ""10000"",""contributionCurrency"": ""USD"",""phoneNumber"": ""{Globals.kycUploadPhone}"",""verificationCode"": ""{Globals.kycUploadCode}"",""kycApiUpload"": ""true""}}";
      var result = new StringContent(requestBody, Encoding.UTF8, "application/json");
      return result;
    }

    private String CreateKycPack(string content)
    {
      // create directory
      var dirPath = Path.Combine(System.Environment.CurrentDirectory, tempPath);
      var txtPath = Path.Combine(System.Environment.CurrentDirectory, txtFile);
      if (!Directory.Exists(dirPath))
      {
        Directory.CreateDirectory(dirPath);
      }

      if (File.Exists(Path.Combine(dirPath, txtFile)))
      {
        File.Delete(Path.Combine(dirPath, txtFile));
      }

      if (File.Exists(txtPath))
      {
        File.Delete(txtPath);
      }

      System.IO.File.WriteAllText(txtPath, content);
      File.Copy(txtPath, Path.Combine(dirPath, txtFile));

      var zipPath = Path.Combine(System.Environment.CurrentDirectory, zipFile);
      if(File.Exists(zipPath))
      {
        File.Delete(zipPath);
      }

      // now we create zip file
      ZipFile.CreateFromDirectory(dirPath, zipPath);
      FileStream fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read);

      // Create a byte array of file stream length
      byte[] pack = new byte[fs.Length];
      fs.Read(pack, 0, System.Convert.ToInt32(fs.Length));
      fs.Close();
      return Convert.ToBase64String(pack);
    }

    public async Task<ValueTuple<bool, string>> UploadKycPack(string content)
    {
      try
      {
        Uri u = new Uri(Globals.kycAPIEndpoint + "/erckycupload");
        // generate zip file
        var kycPack = CreateKycPack(content);
        var httpContent = GenerateHttpContent(kycPack);
        HttpResponseMessage result = await _client.PostAsync(u, httpContent);
        if (result.IsSuccessStatusCode)
        {
          var response = await result.Content.ReadAsStringAsync();
          return (true, response);
        }
        else
        {
          return (false, result.StatusCode.ToString());
        }
      }
      catch (Exception ex)
      {
        return (false, ex.Message);
      }
    }
  }
}
