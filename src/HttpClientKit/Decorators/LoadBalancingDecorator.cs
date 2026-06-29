using HttpClientKit.Discovery;
using HttpClientKit.Exceptions;

namespace HttpClientKit.Decorators;

public sealed class LoadBalancingDecorator(IHttpClient inner, ServiceRegistry registry, IpHealthTracker tracker)
    : HttpClientDecorator(inner)
{
    private readonly Dictionary<string, int> _roundRobinStart = new();

    public override HttpResponse Send(HttpRequest request)
    {
        var host = HttpUrl.GetHost(request.Url);
        if (!registry.TryGetIps(host, out var ips))
        {
            return Inner.Send(request);
        }

        var chosenIp = NextAliveIpByRoundRobin(host, ips);
        if (chosenIp is null)
        {
            throw new HttpRequestFailedException(request.Url);
        }

        var rewritten = request.WithUrl(HttpUrl.ReplaceHostWithIp(request.Url, chosenIp));
        try
        {
            return Inner.Send(rewritten);
        }
        catch (HttpRequestFailedException)
        {
            tracker.MarkFailed(chosenIp);
            throw;
        }
    }

    private string? NextAliveIpByRoundRobin(string host, IReadOnlyList<string> ips)
    {
        var start = _roundRobinStart.GetValueOrDefault(host);
        for (var step = 0; step < ips.Count; step++)
        {
            var index = (start + step) % ips.Count;
            var ip = ips[index];
            if (tracker.IsAlive(ip))
            {
                _roundRobinStart[host] = index + 1;
                return ip;
            }
        }

        return null;
    }
}
