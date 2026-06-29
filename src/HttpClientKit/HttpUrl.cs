namespace HttpClientKit;

public static class HttpUrl
{
    public static string GetHost(string url) => new Uri(url).Host;

    public static string ReplaceHostWithIp(string url, string newHost)
    {
        var builder = new UriBuilder(url) { Host = newHost };
        return builder.Uri.ToString();
    }
}
