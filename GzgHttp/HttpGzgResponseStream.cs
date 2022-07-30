namespace GzgHttp;

public class HttpGzgResponseStream : HttpGzgResponse<Stream> , IDisposable
{

    private bool dispose = false;

    public HttpGzgResponseStream(bool isSuccess, Stream responseContent, string errorMessage , int statusCode) : base(isSuccess, responseContent, errorMessage, statusCode)
    {
    }

    public HttpGzgResponseStream(bool isSuccess, Stream responseContent, int statusCode) : base(isSuccess, responseContent , statusCode)
    {
    }

    public HttpGzgResponseStream(bool isSuccess, string errorMessage, int statusCode) : base(isSuccess, errorMessage , statusCode)
    {
    }


    public void Dispose()
    {
        if (!dispose)
        {
            if(ResponseContent != null)
            {
                ResponseContent.Dispose();
            }
            dispose = true;
            GC.SuppressFinalize(this);
        }
    }
}
