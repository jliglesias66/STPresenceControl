using System.Collections.Specialized;
using System.Linq;
using System.Net;

namespace STPresenceControl.Extensions
{
    public static class HttpExtensions
    {
        public static string GenerateQueryString(this NameValueCollection collection)
        {
            if (collection == null) return null;
            var array = (from key in collection.AllKeys
                         from value in collection.GetValues(key)
                         select string.Format("{0}={1}", WebUtility.UrlEncode(key), WebUtility.UrlEncode(value))).ToArray();
            return string.Join("&", array);
        }
    }
}
