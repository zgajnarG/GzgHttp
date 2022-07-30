namespace GzgHttp;

public class HttpGzgResponse<T>
{
    public readonly bool isSuccess;
    public readonly T responseContent;
    public readonly string errorMessage;
    public readonly int statusCode;

    public HttpGzgResponse(bool isSuccess, T responseContent , string errorMessage , int statusCode )
    {
        this.isSuccess = isSuccess;
        this.responseContent = responseContent;
        this.errorMessage = errorMessage;
        this.statusCode = statusCode;
    }

    public HttpGzgResponse(bool isSuccess, T responseContent , int statusCode)
    {
        this.isSuccess = isSuccess;
        this.responseContent = responseContent;
        this.errorMessage = String.Empty;
        this.statusCode = statusCode;
    }
    public HttpGzgResponse(bool isSuccess, string errorMessage , int statusCode)
    {
        this.isSuccess = isSuccess;
        this.errorMessage = errorMessage;
        this.responseContent = default;
        this.statusCode = statusCode;
    }


}


