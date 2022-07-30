using Newtonsoft.Json;
using System.ComponentModel;
using System.Text;

namespace GzgHttp.Extensions
{
    public static class ExtensionsMethods
    {
        public static string ToDescriptionString(this HttpGzgContentTypes val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static HttpContent GetHttpContent(this HttpGzgContentTypes val, object content)
        {
            if (val == HttpGzgContentTypes.JSON || val == HttpGzgContentTypes.XML)
            {
                string json;
                if (content.GetType() == typeof(string) || content.GetType() == typeof(String))
                {
                    json = (string)content;
                }
                else
                {
                    json = JsonConvert.SerializeObject(content);
                }
                return new StringContent(json, Encoding.UTF8, val.ToDescriptionString());
            }
            else if (val == HttpGzgContentTypes.FORM_URLENCODED)
            {
                return new FormUrlEncodedContent(content as Dictionary<string, string>);
            }
            else
            {
                return new StreamContent(content as Stream);
            }
        }


        public static HttpMethod GetMethod(this HttpGzgMethods method)
        {
            if (method == HttpGzgMethods.POST)
                return HttpMethod.Post;
            else if (method == HttpGzgMethods.DELETE)
                return HttpMethod.Delete;
            else if (method == HttpGzgMethods.PATCH)
                return HttpMethod.Patch;
            else if (method == HttpGzgMethods.PUT)
                return HttpMethod.Put;
            else
                return HttpMethod.Get;
        }

    }
}
