using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KYCAPISample
{
  public class RequestAccessTokenResult
  {
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
  }
  public class KYCReviewer
  {
    private string accessToken = null;
    private HttpClient _client;
    public KYCReviewer()
    {
      _client = new HttpClient();
    }

    /// <summary>
    /// Request access token with provided refresh token
    /// </summary>
    /// <returns>true if access token successfully aquired, false otherwise.</returns>
    public async Task<bool> RequestAccessToken()
    {
      try
      {
        Uri u = new Uri(Globals.b2cRequestEndpoint);
        var dict = new Dictionary<string, string>
      {
        { "client_id", Globals.b2cClientId },
        { "grant_type", Globals.b2cGrantType },
        { "scope", Globals.b2cScope },
        { "redirect_uri", Globals.b2cRedirectUrl },
        { "client_secret", Globals.b2cClientSecret },
        { "refresh_token", Globals.b2cRefreshToken }
      };
        _client.DefaultRequestHeaders.Clear();
        HttpResponseMessage result = await _client.PostAsync(u, new FormUrlEncodedContent(dict));
        if (result.IsSuccessStatusCode)
        {
          var content = await result.Content.ReadAsStringAsync();
          var json = JsonSerializer.Deserialize<RequestAccessTokenResult>(content);
          accessToken = json.AccessToken;

          return true;
        }
        else
        {
          accessToken = string.Empty;
          return false;
        }
      }
      catch (Exception)
      {
        accessToken = string.Empty;
        return false;
      }
    }

    /// <summary>
    /// View a list of kyc packs stored in azure cold storage.
    /// </summary>
    /// <returns></returns>
    public async Task<ValueTuple<bool,string>> List()
    {
      try
      {
        Uri u = new Uri(Globals.kycAPIEndpoint+"/api/kyc/list");
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        HttpResponseMessage result = await _client.GetAsync(u);
        if (result.IsSuccessStatusCode)
        {
          var content = await result.Content.ReadAsStringAsync();
          return (true, content);
        }
        else
        {
          if(result.StatusCode == System.Net.HttpStatusCode.Forbidden)
          {
            // update access token
            await RequestAccessToken();
          }
          return (false, null);
        }
      }
      catch (Exception ex)
      {
        return (false, ex.Message);
      }
    }

    /// <summary>
    /// Download a kyc pack from azure cold storage
    /// </summary>
    /// <param name="id">id/hash of the pack for download</param>
    /// <param name="notarize">will notarise the download operation if true; false otherwise</param>
    /// <returns></returns>
    public async Task<ValueTuple<bool, string>> Download(string id, bool notarize = false)
    {
      try
      {
        Uri u = new Uri(Globals.kycAPIEndpoint + (notarize? $"/api/kyc/download/{id}/blockchain/true": $"/api/kyc/download/{id}"));
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        HttpResponseMessage result = await _client.PostAsync(u, null);
        if (result.IsSuccessStatusCode)
        {
          var content = await result.Content.ReadAsStringAsync();
          return (true, content);
        }
        else
        {
          if (result.StatusCode == System.Net.HttpStatusCode.Forbidden)
          {
            // update access token
            await RequestAccessToken();
          }
          return (false, result.StatusCode.ToString());
        }
      }
      catch (Exception ex)
      {
        return (false, ex.Message);
      }
    }

    /// <summary>
    /// Approve a kyc pack 
    /// </summary>
    /// <param name="id">id of the pack to be approved</param>
    /// <param name="notarize">flag indicates whether the operation will be notarised or not</param>
    /// <returns></returns>
    public async Task<ValueTuple<bool, string>> Approve(string id, bool notarize = false)
    {
      try
      {
        Uri u = new Uri(Globals.kycAPIEndpoint + (notarize ? $"/api/kyc/approve/{id}/blockchain/true" : $"/api/kyc/approve/{id}"));
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        HttpResponseMessage result = await _client.PatchAsync(u, null);
        if (result.IsSuccessStatusCode)
        {
          var content = await result.Content.ReadAsStringAsync();
          return (true, content);
        }
        else
        {
          if (result.StatusCode == System.Net.HttpStatusCode.Forbidden)
          {
            // update access token
            await RequestAccessToken();
          }
          return (false, null);
        }
      }
      catch (Exception ex)
      {
        return (false, ex.Message);
      }
    }
    /// <summary>
    /// Remove a kyc pack from the cold storage
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ValueTuple<bool, string>> Delete(string id)
    {
      try
      {
        Uri u = new Uri(Globals.kycAPIEndpoint + $"/api/kyc/delete/{id}");
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        HttpResponseMessage result = await _client.DeleteAsync(u);
        if (result.IsSuccessStatusCode)
        {
          var content = await result.Content.ReadAsStringAsync();
          return (true, content);
        }
        else
        {
          if (result.StatusCode == System.Net.HttpStatusCode.Forbidden)
          {
            // update access token
            await RequestAccessToken();
          }
          return (false, null);
        }
      }
      catch (Exception ex)
      {
        return (false, ex.Message);
      }
    }
  }
}
