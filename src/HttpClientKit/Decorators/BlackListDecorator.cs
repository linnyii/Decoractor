using HttpClientKit.Exceptions;

namespace HttpClientKit.Decorators;

public sealed class BlackListDecorator(IHttpClient inner, IEnumerable<string> blacklistHosts)
    : HttpClientDecorator(inner)
{
    private readonly HashSet<string> _blacklist = new(blacklistHosts, StringComparer.OrdinalIgnoreCase);

    public static BlackListDecorator FromConfig(IHttpClient inner, string commaSeparatedHosts)
    {
        var hosts = commaSeparatedHosts
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return new BlackListDecorator(inner, hosts);
    }

    public override HttpResponse Send(HttpRequest request)
    {
        var host = HttpUrl.GetHost(request.Url);
        if (_blacklist.Contains(host))
        {
            throw new BlackListedHostException(host);
        }

        return Inner.Send(request);
    }
}
