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
    private string? baseAddress;

    private Uri fullPathRequest
    {
        get
        {
            string? url= this.baseAddress != null ? baseAddress + this.endpoint : endpoint;
            if (url != null)
                return new Uri(url);
            else 
                throw new ArgumentNullException("endpoint empty");
        }
    }

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
        this.RemoveBaseAddress();

    }
    #endregion




    #region Gestion des headers & httpclient


    public HttpGzgClient SetBaseAddress(string baseUri)
    {
        this.baseAddress = baseUri ;
        return this;
    }
    public void RemoveBaseAddress()
    {
        this.baseAddress = null;
    }

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
    public HttpGzgResponse<String> Get()
    {
        using var response = this.Send(HttpGzgMethods.GET);
        return GetHttpGzgResponse(response);
    }
    public async Task<HttpGzgResponse<String>> GetAsync()
    {
        using var response = await this.SendAsync(HttpGzgMethods.GET);
        return await GetHttpGzgResponseAsync(response);
    }

    public HttpGzgResponse<String> Post(object body, HttpGzgContentTypes content)
    {
        using var response =  this.Send(HttpGzgMethods.POST, body, content);
        return GetHttpGzgResponse(response);
    }

    public async Task<HttpGzgResponse<String>> PostAsync(object body, HttpGzgContentTypes content)
    {
        using var response = await this.SendAsync(HttpGzgMethods.POST, body, content);
        return await GetHttpGzgResponseAsync(response);
    }

    

    public HttpGzgResponse<String> Put(object body, HttpGzgContentTypes content)
    {
        using var response = this.Send(HttpGzgMethods.PUT, body, content);
        return GetHttpGzgResponse(response);
    }

    public async Task<HttpGzgResponse<String>> PutAsync(object body, HttpGzgContentTypes content)
    {
        using var response = await this.SendAsync(HttpGzgMethods.PUT, body, content);
        return await GetHttpGzgResponseAsync(response);
    }
    public HttpGzgResponse<String> Patch(object body, HttpGzgContentTypes content)
    {
        using var response = this.Send(HttpGzgMethods.PATCH, body, content);
        return GetHttpGzgResponse(response);
    }

    public async Task<HttpGzgResponse<String>> PatchAsync(object body, HttpGzgContentTypes content)
    {
        using var response = await this.SendAsync(HttpGzgMethods.PATCH, body, content);
        return await GetHttpGzgResponseAsync(response);
    }

    public HttpResponseMessage Send(HttpGzgMethods method, object? body = null, HttpGzgContentTypes? content = null)
    {

        using HttpRequestMessage request = new() { RequestUri =this.fullPathRequest, Method = method.GetMethod() };

        if (content != null && body != null)
        {
            request.Content = content?.GetHttpContent(body);
        }
        return this._httpClient.Send(request);
    }


    public async Task<HttpResponseMessage> SendAsync(HttpGzgMethods method, object? body = null, HttpGzgContentTypes? content = null)
    {
        using HttpRequestMessage request = new() { RequestUri =this.fullPathRequest, Method = method.GetMethod() };

        if (content != null && body != null)
        {
            request.Content = content?.GetHttpContent(body);
        }
        return await this._httpClient.SendAsync(request);
    }


    public HttpGzgResponse<String> Delete()
    {
        using var response = this.Send(HttpGzgMethods.DELETE);
        return GetHttpGzgResponse(response);
    }

    public async Task<HttpGzgResponse<String>> DeleteAsync()
    {
        using var response = await this.SendAsync(HttpGzgMethods.DELETE);
        return await GetHttpGzgResponseAsync(response);
    }

    public HttpGzgResponse<String> PostJson(string json)
    {
        using HttpRequestMessage requestMessage = new() { RequestUri = this.fullPathRequest, Method = HttpMethod.Post, Content = new StringContent(json, Encoding.UTF8, "application/json") };
        using var response =  _httpClient.Send(requestMessage);
        return GetHttpGzgResponse(response);
    }
    #endregion

    #region Partie custom 


    public HttpGzgResponseStream SendAndGetResponseStream(HttpGzgMethods method = HttpGzgMethods.GET, object? body = null , HttpGzgContentTypes? content = null )
    {
        using var response = this.Send(method, body, content);
        return GetHttpGzgResponseDisposable(response);
    }


    public async Task<HttpGzgResponseStream> SendAndGetResponseStreamAsync(HttpGzgMethods method = HttpGzgMethods.GET, object? body = null, HttpGzgContentTypes? content = null)
    {
        using var response = await this.SendAsync(method, body, content);
        return GetHttpGzgResponseDisposable(response);
    }


    public HttpGzgResponse<T> SendAndParse<T>(HttpGzgMethods method = HttpGzgMethods.GET, object? body = null, HttpGzgContentTypes? content = null , bool parseResultIfError = false)
    {
        using var response = this.Send(method, body, content);
        return GetHttpGzgResponseParse<T>(response, parseResultIfError);
    }

    public async Task<HttpGzgResponse<T>> SendAndParseAsync<T>(HttpGzgMethods method = HttpGzgMethods.GET, object? body = null, HttpGzgContentTypes? content = null, bool parseResultIfError = false)
    {
        using var response = await this.SendAsync(method, body, content);
        return await  GetHttpGzgResponseParseAsync<T>(response, parseResultIfError);
    }


    public async Task<HttpGzgResponse<T>> PostJsonAndParseAsync<T>(string json, bool parseResultIfError = false)
    {
        using HttpResponseMessage response = await _httpClient.PostAsync(this.endpoint, new StringContent(json, Encoding.UTF8, "application/json"));
        return await GetHttpGzgResponseParseAsync<T>(response, parseResultIfError);
    }

    public HttpGzgResponse<T> PostJsonAndParse<T>(string json , bool parparseResultIfError = false)
    {
        using HttpRequestMessage request = new() { RequestUri = this.fullPathRequest, Method = HttpMethod.Get, Content = new StringContent(json, Encoding.UTF8, "application/json") };
        using var response = _httpClient.Send(request);
        return GetHttpGzgResponseParse<T>(response, parparseResultIfError);
    }

    private static HttpGzgResponse<T> GetHttpGzgResponseParse<T>(HttpResponseMessage response , bool parparseResultIfError)
    {
        int statusCode = (int)response.StatusCode;
        HttpGzgResponse<T> gzgResp;
        if (response.IsSuccessStatusCode && response.Content != null)
        {
            gzgResp = new HttpGzgResponse<T>(true, JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result), statusCode);
        }
        else if (response.Content != null)
        {
            gzgResp = parparseResultIfError ? 
             new HttpGzgResponse<T>(false, JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result) , "" , statusCode) :
             new HttpGzgResponse<T>(false, response.Content.ReadAsStringAsync().Result, statusCode);


        }
        else
        {
            gzgResp = new HttpGzgResponse<T>(false, "No content in response", statusCode);
        }
        gzgResp.Headers = response.Headers.ToDictionary(x => x.Key, x => x.Value);
        return gzgResp;
    }

    private static async Task<HttpGzgResponse<T>> GetHttpGzgResponseParseAsync<T>(HttpResponseMessage response, bool parparseResultIfError)
    {
        int statusCode = (int)response.StatusCode;
        HttpGzgResponse<T> gzgResp;
       
        if (response.IsSuccessStatusCode && response.Content != null)
        {
            gzgResp = new HttpGzgResponse<T>(true, JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync()), statusCode);
        }
        else if (response.Content != null)
        {
            gzgResp = parparseResultIfError ?
            new HttpGzgResponse<T>(false, JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync()), "", statusCode) :
            new HttpGzgResponse<T>(false, await response.Content.ReadAsStringAsync(), statusCode);

        }
        else
        {
            gzgResp = new HttpGzgResponse<T>(false, "No content in response", statusCode);
        }
        gzgResp.Headers = response.Headers.ToDictionary(x => x.Key, x => x.Value);
        return gzgResp;
    }

    private static HttpGzgResponseStream GetHttpGzgResponseDisposable(HttpResponseMessage response)
    {
        int statusCode = (int)response.StatusCode;
        HttpGzgResponseStream gzgResp;
        if (response.IsSuccessStatusCode)
        {
            MemoryStream ms = new MemoryStream();
            response.Content.ReadAsStream().CopyTo(ms);
            gzgResp = new HttpGzgResponseStream(true, ms, statusCode);
        }
        else
        {
            gzgResp = new HttpGzgResponseStream(false, response.Content.ReadAsStringAsync().Result, statusCode);
        }
        gzgResp.Headers = response.Headers.ToDictionary(x => x.Key, x => x.Value);
        return gzgResp;
    }
    
    private static HttpGzgResponse<String> GetHttpGzgResponse(HttpResponseMessage response)
    {
        int statusCode = (int)response.StatusCode;
        HttpGzgResponse<String> gzgResp;

        if (response.IsSuccessStatusCode)
        {
            gzgResp = new HttpGzgResponse<String>(true, responseContent : response.Content == null ? String.Empty :response.Content.ReadAsStringAsync().Result, statusCode);
        }
        else if (response.Content != null)
        {
            gzgResp =  new HttpGzgResponse<String>(false, errorMessage : response.Content.ReadAsStringAsync().Result, statusCode);
        }
        else
        {
            gzgResp = new HttpGzgResponse<String>(false, errorMessage: "No content in response", statusCode);
        }
        gzgResp.Headers = response.Headers.ToDictionary(x => x.Key, x => x.Value);
        return gzgResp;
    }

    private static async Task<HttpGzgResponse<String>> GetHttpGzgResponseAsync(HttpResponseMessage response)
    {
        int statusCode = (int)response.StatusCode;
        HttpGzgResponse<String> gzgResp;

        if (response.IsSuccessStatusCode)
        {
            gzgResp = new HttpGzgResponse<String>(true, responseContent: response.Content == null ? String.Empty : await response.Content.ReadAsStringAsync(), statusCode);
        }
        else if (response.Content != null)
        {
            gzgResp = new HttpGzgResponse<String>(false, errorMessage: await response.Content.ReadAsStringAsync(), statusCode);
        }
        else
        {
            gzgResp = new HttpGzgResponse<String>(false, errorMessage: "No content in response", statusCode);
        }
        gzgResp.Headers = response.Headers.ToDictionary(x => x.Key, x => x.Value);
        return gzgResp;
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

