
using System.ComponentModel;

namespace GzgHttp.Extensions;

public enum HttpGzgContentTypes
{
    [Description("application/x-www-form-urlencoded")]
    FORM_URLENCODED,
    [Description("application/json")]
    JSON,
    [Description("application/octet-stream")]
    FILE_STREAM,
    [Description("application/xml")]
    XML,
    [Description("application/zip")]
    ZIP,
    [Description("application/pdf")]
    PDF,
    [Description("image/gif")]
    GIF,
    [Description("image/jpeg")]
    JPEG,
    [Description("image/png")]
    PNG
}
