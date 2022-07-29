namespace GzgHttp;

public class HttpGzgResponse<T>
{
    public readonly bool isSuccess;
    public readonly T responseContent;
    public readonly string errorMessage;

    public HttpGzgResponse(bool isSuccess, T responseContent)
    {
        this.isSuccess = isSuccess;
        this.responseContent = responseContent;
    }
    public HttpGzgResponse(bool isSuccess, string errorMessage)
    {
        this.isSuccess = isSuccess;
        this.errorMessage = errorMessage;
    }
}


