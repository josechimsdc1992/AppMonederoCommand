using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Entities.Config
{
    public class IMDURL
    {
        public static string NormalizeURL(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            if (!url.EndsWith("/"))
            {
                return url + "/";
            }

            return url;
        }

        public static string NormalizeEndPoint(string segment)
        {
            if (string.IsNullOrEmpty(segment))
            {
                return string.Empty;
            }

            if (segment.StartsWith("/"))
            {
                segment = segment.Substring(1);
            }

            return segment;
        }

        public static string AddSegment(string baseUrl, string segment)
        {
            string normalizedBaseUrl = NormalizeURL(baseUrl);

            if (string.IsNullOrEmpty(segment))
            {
                return baseUrl;
            }

            if (segment.StartsWith("/"))
            {
                segment = segment.Substring(1);
            }

            return normalizedBaseUrl + segment;
        }

    }
}

