
using System.ComponentModel;

namespace GzgHttp.Enums;

public enum HttpGzgContentTypes
{
    [Description("application/x-www-form-urlencoded")]
    FORM_URLENCODED,
    [Description("application/json")]
    JSON,
    [Description("application/octet-stream")]
    STREAM,
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
    PNG,
    [Description("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    DOCX,
    [Description("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    XLSX,
    [Description("application/vnd.openxmlformats-officedocument.presentationml.presentation")]
    PPTX

}
