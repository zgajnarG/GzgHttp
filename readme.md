# GzgHttp library class .NET 6

This class library allows to have an overlay of the basic HttpClient (System.Net.Http).

The purpose of this library is to be able to make http calls faster and to control the results more easily.

Most of the request management is done via the fluent interface (For more information : https://en.wikipedia.org/wiki/Fluent_interface)

# Usage

## Initialisation
```
using HttpGzgClient client = new("https://test.com/test");
```
or
```
using HttpGzgClient client = new();
client.SetBaseAddress("https://base.com").SetEndpoint("/test");
```

## Add headers

### Custom headers
```
using HttpGzgClient client = new("https://test.com/test");
client.AddHeader("test1", "value1").AddHeader("test2", "value2");
```
or 
```
using HttpGzgClient client = new("https://test.com/test");
Dictionary<string, string> headers = new()
{
    { "test1", "value1" },
    { "test2", "value2" }
};
client.AddHeaders(headers);
```
### Authorization header

#### Bearer
```
using HttpGzgClient client = new("https://test.com/test");
client.AddAuthorizationBearer("token");
```
#### Custom  
```
using HttpGzgClient client = new("https://test.com/test");
client.AddAuthorization("Scheme You Want" , "token");
```

### Accept

#### One
```
using HttpGzgClient client = new("https://test.com/test");
client.Accept(HttpGzgContentTypes.JSON);
```
or
```
using HttpGzgClient client = new("https://test.com/test");
client.Accept("application/json");
```

#### List
```
using HttpGzgClient client = new("https://test.com/test");
client.Accept(new List<HttpGzgContentTypes> { HttpGzgContentTypes.JSON , HttpGzgContentTypes.XML } );
```
or
```
using HttpGzgClient client = new("https://test.com/test");
client.Accept(new List<String> {"application/json", "application/xml"});
```


## Send Request & Handle Response

### GET
#### Without parsing response content
```
using HttpGzgClient client = new("https://test.com/test");
HttpGzgResponse<string> response = client.Get();
if (response.IsSuccess)
{
    /// handle response
}
```
#### Parsing response content

Json to model (T could be any class except Stream )
```
using HttpGzgClient client = new("https://test.com/test");
HttpGzgResponse<T> response = client.GetWithResult<T>();
if (response.IsSuccess)
{
    T reponseParsed = reponse.ResponseContent;
    ...
}
```

STREAM ( file , image , ...)

```
using HttpGzgClient client = new("https://test.com/test");
using HttpGzgResponseStream response = client.GetWithStreamResult();
if (response.IsSuccess)
{
    Stream responseStream = reponse.ResponseContent;
    ...
}
```
#### Async functions
```
using HttpGzgClient client = new("https://test.com/test");
HttpGzgResponse<string> response = client.GetAsync();
HttpGzgResponse<T> response = await client.GetWithResultAsync<T>();
using HttpGzgResponseStream response = await client.GetWithStreamResultAsync();
```


### DELETE
Same as GET but without "WithStreamResult" methods

```
using HttpGzgClient client = new("https://test.com/test");
HttpGzgResponse<string> response = client.Delete();
HttpGzgResponse<string> response = await client.DeleteAsync();
HttpGzgResponse<T> response = client.DeleteWithResult<T>();
HttpGzgResponse<T> response = await client.DeleteWithResultAsync<T>();
```

### POST & PATCH & PUT
These methods work in the same way but only the name of the method differs. ( PostWithResult , PatchWithResult , PutWithResult, ...)

JSON
```
Model m = new();
...

using HttpGzgClient client = new("https://test.com/test");
HttpGzgResponse<string> response = client.Post(m,  HttpGzgContentTypes.JSON);
```
or 

```
string json = "{json here}";
using HttpGzgClient client = new("https://test.com/test");
HttpGzgResponse<string> response = client.Post(json,  HttpGzgContentTypes.JSON);
```
It works too with "WithResult" methods or "WithStreamResult".

```
string json = "{json here}";
using HttpGzgClient client = new("https://test.com/test");
HttpGzgResponse<T> response = client.PostWithResult<T>(json,  HttpGzgContentTypes.JSON);
```

Files or images (Works with all stream)
```
using FileStream fs= new(...);
using HttpGzgClient client = new("https://test.com/test");
HttpGzgResponse<T> response = client.PostWithResult<T>(fs,  HttpGzgContentTypes.STREAM);
```
If you know file extension 

```
using MemoryStream ms= new(...);
using HttpGzgClient client = new("https://test.com/test");
client.PostWithResult<T>(ms,  HttpGzgContentTypes.PDF);
client.PostWithResult<T>(ms,  HttpGzgContentTypes.DOCX);
client.PostWithResult<T>(ms,  HttpGzgContentTypes.PNG);
```

XML

it works with a string or an object whose ToString() method returns an xml string 

```
String xml = "xml here";
using HttpGzgClient client = new("https://test.com/test");
HttpGzgResponse<string> response = client.Post(xml,  HttpGzgContentTypes.XML);
```

or 
```
///model.ToString() return an xml string
Model xml = new();

using HttpGzgClient client = new("https://test.com/test");
HttpGzgResponse<string> response = client.Post(xml,  HttpGzgContentTypes.XML);
```


## Exemples

### Send JSON and parse response

```
Model m = new();

using HttpGzgClient client = new("https://test.com/test");

HttpGzgResponse<List<ModelResponses>> response = client.AddAuthorizationBearer("token")
    .AddHeader("test","value")
    .PostWithResult<List<ModelResponse>>(m,HttpGzgContentTypes.JSON);

if(response.IsSuccess){
    int recordsSize = response.ResponseContent.Count;
    ....
}else{
    ....
}
```

### Get file and create local file


```
using HttpGzgClient client = new("https://test.com/file");
using HttpGzgResponseStream response = client.GetWithStreamResult();

if(response.IsSuccess){
    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "newFile.pdf");
    using FileStream fileStream = File.Create(filePath);
    resp.ResponseContent.Seek(0, SeekOrigin.Begin);
    resp.ResponseContent.CopyTo(fileStream);
}else{
    Console.WriteLine($"error : {response.ErrorMessage}");
}
```

### Send multiple request


```
using HttpGzgClient client = new();
client.SetBaseAddress("https://base.com");

HttpGzgResponse<ModelA> responseA = client.SetEndpoint('/a')
    .AddHeader("type","A")
    .AddAuthorizationBearer("token")
    .GetWithResult<ModelA>();

HttpGzgResponse<ModelB> responseB = client.ClearHeaders(clearAuthorization : true)
    .SetEndpoint('/b')
    .AddHeader("type","B")
    .GetWithResult<ModelB>();
```