using GzgHttp.Extensions;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace GzgHttp;

public class HttpGzgClient : IDisposable
{
    #region Initialisation de la classe + getter setter

    private readonly HttpClient _httpClient;
    private bool dispose = false;
    private string endpoint;

    public HttpGzgClient(string endpoint)
    {
        _httpClient = new HttpClient();
        this.endpoint = endpoint;
    }

    public HttpClient GetClient()
    {
        return this._httpClient;
    }

    public void ChangeEndpoint(string endpoint)
    {
        this.endpoint = endpoint;
    }

    #endregion




    #region Gestion des headers
    public HttpGzgClient AddHeaders(Dictionary<string, string> headers)
    {

        foreach (string headerName in headers.Keys)
        {
            this._httpClient.DefaultRequestHeaders.Add(headerName, headers[headerName]);
        }

        return this;
    }

    public HttpGzgClient AddHeader(string key, string value)
    {
        this._httpClient.DefaultRequestHeaders.Add(value, key);
        return this;
    }

    public HttpGzgClient Accept(HttpGzgContentTypes type)
    {
        this._httpClient.DefaultRequestHeaders.Accept.Clear();
        this._httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(type.ToDescriptionString()));
        return this;
    }

    public HttpGzgClient Accept(string type)
    {
        this._httpClient.DefaultRequestHeaders.Accept.Clear();
        this._httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(type));
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
    #endregion


    #region Gestion d'envoie de requêtes
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



    public async Task<HttpGzgResponse<T>> PostJsonAsyncAndParse<T>(string json)
    {
        using HttpResponseMessage response = await _httpClient.PostAsync(this.endpoint, new StringContent(json, Encoding.UTF8, "application/json"));
        if (response.IsSuccessStatusCode && response.Content != null)
        {
            return new HttpGzgResponse<T>(true, (T)Convert.ChangeType(await response.Content.ReadAsStringAsync(), typeof(T)));
        }
        else if (response.Content != null)
        {
            return new HttpGzgResponse<T>(false, await response.Content.ReadAsStringAsync());

        }
        return new HttpGzgResponse<T>(false, "No content in response");
    }

    public bool TryPostJsonAndParse<T>(string json, out T value)
    {
        value = default;
        using HttpRequestMessage request = new() { RequestUri = new Uri(this.endpoint), Method = HttpMethod.Get, Content = new StringContent(json, Encoding.UTF8, "application/json") };
        using var response = _httpClient.Send(request);
        if (response.IsSuccessStatusCode )
        {
            value = JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            return true;
        }
        return false;
    }

    public async Task<Stream> GetStreamAsync()
    {
        return await this._httpClient.GetStreamAsync(this.endpoint);
    }



    public async Task<T> GetAsyncAndParse<T>()
    {
        string response = await this._httpClient.GetStringAsync(this.endpoint);
        return (T)Convert.ChangeType(response, typeof(T));
    }

    public bool TryGetAndParse<T>(out T parseResult)
    {
        parseResult = default;
        using HttpRequestMessage request = new() { RequestUri = new Uri(this.endpoint), Method = HttpMethod.Get };
        using var response = this._httpClient.Send(request);
        if (response.IsSuccessStatusCode)
        {
             parseResult = JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
             return true;
        }
        return false;
    }

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

