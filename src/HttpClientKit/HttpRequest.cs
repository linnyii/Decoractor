namespace HttpClientKit;

public sealed record HttpRequest(string Url)
{
    public HttpRequest WithUrl(string url) => this with { Url = url };
}
