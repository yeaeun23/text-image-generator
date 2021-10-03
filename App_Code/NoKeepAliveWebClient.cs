using System;
using System.Net;

public class NoKeepAliveWebClient : WebClient
{
    protected override WebRequest GetWebRequest(Uri address)
    {
        WebRequest request = base.GetWebRequest(address);

        if (request is HttpWebRequest)
            ((HttpWebRequest)request).KeepAlive = false;

        return request;
    }
}
