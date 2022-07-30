namespace GzgHttp;

public class HttpGzgResponseDisposable<T> : HttpGzgResponse<T> , IDisposable where T : IDisposable
{

    private bool dispose = false;

    public HttpGzgResponseDisposable(bool isSuccess, T responseContent, string errorMessage , int statusCode) : base(isSuccess, responseContent, errorMessage, statusCode)
    {
    }

    public HttpGzgResponseDisposable(bool isSuccess, T responseContent, int statusCode) : base(isSuccess, responseContent , statusCode)
    {
    }

    public HttpGzgResponseDisposable(bool isSuccess, string errorMessage, int statusCode) : base(isSuccess, errorMessage , statusCode)
    {
    }


    public void Dispose()
    {
        if (!dispose)
        {
            if(responseContent != null)
            {
                responseContent.Dispose();
            }
            dispose = true;
        }
    }
}
