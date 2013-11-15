using System;
using System.Net;

namespace Dadjai.WebSpider
{
    public class GZipBrowser : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.AutomaticDecompression = DecompressionMethods.GZip |
                                             DecompressionMethods.Deflate;
            return request;
        }
    }
}