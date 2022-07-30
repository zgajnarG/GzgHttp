namespace GzgHttp;

public class HttpGzgResponse<T>
{
    public readonly bool IsSuccess;
    public readonly T ResponseContent;
    public readonly string ErrorMessage;
    public readonly int StatusCode;
    public Dictionary<string , IEnumerable<string>> Headers;

    public HttpGzgResponse(bool isSuccess, T responseContent , string errorMessage , int statusCode )
    {
        this.IsSuccess = isSuccess;
        this.ResponseContent = responseContent;
        this.ErrorMessage = errorMessage;
        this.StatusCode = statusCode;
    }
    public HttpGzgResponse(bool isSuccess, T responseContent , int statusCode)
    {
        this.IsSuccess = isSuccess;
        this.ResponseContent = responseContent;
        this.ErrorMessage = String.Empty;
        this.StatusCode = statusCode;
    }
    public HttpGzgResponse(bool isSuccess, string errorMessage , int statusCode)
    {
        this.IsSuccess = isSuccess;
        this.ErrorMessage = errorMessage;
        this.ResponseContent = default;
        this.StatusCode = statusCode;
    }
}


