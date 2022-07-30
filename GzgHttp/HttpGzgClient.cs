using GzgHttp.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace GzgHttp;

public class HttpGzgClient : IDisposable
{
    #region Initialisation de la classe + getter setter

    private readonly HttpClient _httpClient;
    private string? endpoint;

    private bool dispose = false;

    public HttpGzgClient(string endpoint)
    {
        _httpClient = new HttpClient();
        this.endpoint = endpoint;
    }

    public HttpGzgClient()
    {
        _httpClient = new HttpClient();
    }

    public HttpGzgClient SetEndpoint(string endpoint)
    {
        this.endpoint = endpoint;
        return this;

    }


    public void Clear()
    {
        this.endpoint = String.Empty;
        this.ClearHeaders();
        this.RemoveAuthorization();
    }
    #endregion




    #region Gestion des headers

    public HttpGzgClient ClearHeaders()
    {
        this._httpClient.DefaultRequestHeaders.Clear();
        return this;
    }
    
    public HttpGzgClient RemoveHeader(string name)
    {
        this._httpClient.DefaultRequestHeaders.Remove(name);
        return this;
    }

    public HttpGzgClient AddHeaders(Dictionary<string, string> headers)
    {

        foreach (string headerName in headers.Keys)
        {
            this.AddHeader(headerName, headers[headerName]);
        }

        return this;
    }

    public HttpGzgClient AddHeader(string key, string value)
    {
        if(!_httpClient.DefaultRequestHeaders.TryAddWithoutValidation(key, value))
        {
            throw new FormatException($"cant add header : {key}");
        }
        return this;
    }

    public HttpGzgClient Accept(IEnumerable<HttpGzgContentTypes> types)
    {
        foreach(var type in types)
        {
            this._httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(type.ToDescriptionString()));
        }
        return this;
    }

    public HttpGzgClient Accept(IEnumerable<string> types)
    {
        foreach (var type in types)
        {
            this._httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(type));
        }
        return this;
    }

    public HttpGzgClient Accept(HttpGzgContentTypes type)
    {
        this._httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(type.ToDescriptionString()));
        return this;
    }

    public HttpGzgClient Accept(string type)
    {
       
        this._httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(type));
        return this;
    }

    public HttpGzgClient RemoveAccept()
    {
        this._httpClient.DefaultRequestHeaders.Accept.Clear();
        return this;
    }

    public HttpGzgClient AddAuthorization(string scheme, string token)
    {
        this._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
        return this;
    }
    public HttpGzgClient AddAuthorizationBearer(string token)
    {
        this._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return this;
    }

    public HttpGzgClient RemoveAuthorization()
    {
        this._httpClient.DefaultRequestHeaders.Authorization = null;
        return this;
    }

    #endregion


    #region Gestion d'envoie de requêtes

    #region Partie client basique
    public HttpResponseMessage Get()
    {
        return this.Send(HttpGzgMethods.GET);
    }
    public async Task<HttpResponseMessage> GetAsync()
    {
        return await this.SendAsync(HttpGzgMethods.GET);
    }

    public HttpResponseMessage Post(object body, HttpGzgContentTypes content)
    {
        return this.Send(HttpGzgMethods.POST, body, content);
    }

    public async Task<HttpResponseMessage> PostAsync(object body, HttpGzgContentTypes content)
    {
        return await this.SendAsync(HttpGzgMethods.POST, body, content);
    }

    

    public HttpResponseMessage Put(object body, HttpGzgContentTypes content)
    {
        return this.Send(HttpGzgMethods.PUT, body, content);
    }

    public async Task<HttpResponseMessage> PutAsync(object body, HttpGzgContentTypes content)
    {
        return await this.SendAsync(HttpGzgMethods.PUT, body, content);
    }
    public HttpResponseMessage Patch(object body, HttpGzgContentTypes content)
    {
        return this.Send(HttpGzgMethods.PATCH, body, content);
    }

    public async Task<HttpResponseMessage> PatchAsync(object body, HttpGzgContentTypes content)
    {
        return await this.SendAsync(HttpGzgMethods.PATCH, body, content);
    }

    public HttpResponseMessage Send(HttpGzgMethods method, object? body = null, HttpGzgContentTypes? content = null)
    {
        using HttpRequestMessage request = new() { RequestUri = new Uri(this.endpoint), Method = method.GetMethod() };

        if (content != null && body != null)
        {
            request.Content = content?.GetHttpContent(body);
        }
        return this._httpClient.Send(request);
    }


    public async Task<HttpResponseMessage> SendAsync(HttpGzgMethods method, object? body = null, HttpGzgContentTypes? content = null)
    {
        using HttpRequestMessage request = new() { RequestUri = new Uri(this.endpoint), Method = method.GetMethod() };

        if (content != null && body != null)
        {
            request.Content = content?.GetHttpContent(body);
        }
        return await this._httpClient.SendAsync(request);
    }


    public HttpResponseMessage Delete()
    {
        return this.Send(HttpGzgMethods.DELETE);
    }

    public async Task<HttpResponseMessage> DeleteAsync()
    {
        return await this.SendAsync(HttpGzgMethods.DELETE);
    }

    public HttpResponseMessage PostJson(string json)
    {
        using HttpRequestMessage requestMessage = new() { RequestUri = new Uri(this.endpoint), Method = HttpMethod.Post, Content = new StringContent(json, Encoding.UTF8, "application/json") };
        return _httpClient.Send(requestMessage);
    }
    #endregion

    #region Partie custom 


    public HttpGzgResponseDisposable<Stream> SendAndGetResponseStream(HttpGzgMethods method = HttpGzgMethods.GET, object? body = null , HttpGzgContentTypes? content = null)
    {
        using var response = this.Send(method, body, content);
        return GetHttpGzgResponseDisposable(response);
    }


    public async Task<HttpGzgResponseDisposable<Stream>> SendAndGetResponseStreamAsync(HttpGzgMethods method = HttpGzgMethods.GET, object? body = null, HttpGzgContentTypes? content = null)
    {
        using var response = await this.SendAsync(method, body, content);
        return GetHttpGzgResponseDisposable(response);
    }


    public HttpGzgResponse<T> SendAndParse<T>(HttpGzgMethods method = HttpGzgMethods.GET, object? body = null, HttpGzgContentTypes? content = null)
    {
        using var response = this.Send(method, body, content);
        return GetHttpGzgResponseParse<T>(response);
    }

    public async Task<HttpGzgResponse<T>> SendAndParseAsync<T>(HttpGzgMethods method = HttpGzgMethods.GET, object? body = null, HttpGzgContentTypes? content = null)
    {
        using var response = await this.SendAsync(method, body, content);
        return await  GetHttpGzgResponseParseAsync<T>(response);
    }


    public async Task<HttpGzgResponse<T>> PostJsonAndParseAsync<T>(string json)
    {
        using HttpResponseMessage response = await _httpClient.PostAsync(this.endpoint, new StringContent(json, Encoding.UTF8, "application/json"));
        return await GetHttpGzgResponseParseAsync<T>(response);
    }

    public HttpGzgResponse<T> PostJsonAndParse<T>(string json)
    {
        using HttpRequestMessage request = new() { RequestUri = new Uri(this.endpoint), Method = HttpMethod.Get, Content = new StringContent(json, Encoding.UTF8, "application/json") };
        using var response = _httpClient.Send(request);
        return GetHttpGzgResponseParse<T>(response);
    }

    private static HttpGzgResponse<T> GetHttpGzgResponseParse<T>(HttpResponseMessage response)
    {
        int statusCode = (int)response.StatusCode;
        if (response.IsSuccessStatusCode && response.Content != null)
        {
            return new HttpGzgResponse<T>(true, JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result), statusCode);
        }
        else if (response.Content != null)
        {
            return new HttpGzgResponse<T>(false, response.Content.ReadAsStringAsync().Result, statusCode);

        }
        return new HttpGzgResponse<T>(false, "No content in response", statusCode);
    }

    private static async Task<HttpGzgResponse<T>> GetHttpGzgResponseParseAsync<T>(HttpResponseMessage response)
    {
        int statusCode = (int)response.StatusCode;
        if (response.IsSuccessStatusCode && response.Content != null)
        {
            return new HttpGzgResponse<T>(true, JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync()), statusCode);
        }
        else if (response.Content != null)
        {
            return new HttpGzgResponse<T>(false, await response.Content.ReadAsStringAsync(), statusCode);

        }
        return new HttpGzgResponse<T>(false, "No content in response", statusCode);
    }

    private static HttpGzgResponseDisposable<Stream> GetHttpGzgResponseDisposable(HttpResponseMessage response)
    {
        int statusCode = (int)response.StatusCode;
        if (response.IsSuccessStatusCode)
        {
            MemoryStream ms = new MemoryStream();
            response.Content.ReadAsStream().CopyTo(ms);
            return new HttpGzgResponseDisposable<Stream>(true, ms, statusCode);
        }
        return new HttpGzgResponseDisposable<Stream>(false, response.Content.ReadAsStringAsync().Result, statusCode);
    }
    #endregion
    #endregion


    public void Dispose()
    {
        if (!dispose)
        {
            _httpClient.Dispose();
            dispose = true;
            GC.SuppressFinalize(this);
        }
    }
}

