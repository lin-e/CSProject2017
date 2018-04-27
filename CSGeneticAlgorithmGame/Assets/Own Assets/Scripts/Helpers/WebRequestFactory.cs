using System.Net;

public class WebRequestFactory
{
    public WebProxy mainProxy;
    public CookieContainer mainContainer;
    public string userAgent;
    public WebRequestFactory(WebProxy proxyToUse, CookieContainer containerToUse, string userAgentToUse = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36")
    {
        mainProxy = proxyToUse;
        mainContainer = containerToUse;
        userAgent = userAgentToUse;
    }
    public HttpWebRequest WebRequestGET(string targetURL, string[][] requestHeaders, string referHeader = null)
    {
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(targetURL);
        webRequest.Proxy = mainProxy;
        webRequest.UserAgent = userAgent;
        webRequest.CookieContainer = mainContainer;
        webRequest.Method = "GET";
        if (referHeader != null)
        {
            webRequest.Referer = referHeader;
        }
        foreach (string[] headerPair in requestHeaders)
        {
            webRequest.Headers.Add(headerPair[0], headerPair[1]);
        }
        return webRequest;
    }
    public HttpWebRequest WebRequestPOST(string targetURL, string[][] requestHeaders, byte[] postData, string contentType, string referHeader = null)
    {
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(targetURL);
        webRequest.Proxy = mainProxy;
        webRequest.UserAgent = userAgent;
        webRequest.CookieContainer = mainContainer;
        webRequest.Method = "POST";
        webRequest.ContentType = contentType;
        if (referHeader != null)
        {
            webRequest.Referer = referHeader;
        }
        foreach (string[] headerPair in requestHeaders)
        {
            webRequest.Headers.Add(headerPair[0], headerPair[1]);
        }
        webRequest.ContentLength = postData.Length;
        using (var requestStream = webRequest.GetRequestStream())
        {
            requestStream.Write(postData, 0, postData.Length);
        }
        return webRequest;
    }
    public HttpWebRequest WebRequestDELETE(string targetURL, string[][] requestHeaders, string referHeader = null)
    {
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(targetURL);
        webRequest.Proxy = mainProxy;
        webRequest.UserAgent = userAgent;
        webRequest.CookieContainer = mainContainer;
        webRequest.Method = "DELETE";
        if (referHeader != null)
        {
            webRequest.Referer = referHeader;
        }
        foreach (string[] headerPair in requestHeaders)
        {
            webRequest.Headers.Add(headerPair[0], headerPair[1]);
        }
        return webRequest;
    }
    public HttpWebRequest WebRequestPUT(string targetURL, string[][] requestHeaders, byte[] postData, string contentType, string referHeader = null)
    {
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(targetURL);
        webRequest.Proxy = mainProxy;
        webRequest.UserAgent = userAgent;
        webRequest.CookieContainer = mainContainer;
        webRequest.Method = "PUT";
        webRequest.ContentType = contentType;
        if (referHeader != null)
        {
            webRequest.Referer = referHeader;
        }
        foreach (string[] headerPair in requestHeaders)
        {
            webRequest.Headers.Add(headerPair[0], headerPair[1]);
        }
        webRequest.ContentLength = postData.Length;
        using (var requestStream = webRequest.GetRequestStream())
        {
            requestStream.Write(postData, 0, postData.Length);
        }
        return webRequest;
    }
}
