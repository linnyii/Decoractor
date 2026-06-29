using HttpClientKit.Discovery;
using HttpClientKit.Exceptions;

namespace HttpClientKit.Decorators;

public sealed class ServiceDiscoveryDecorator(IHttpClient inner, ServiceRegistry registry, IpHealthTracker tracker)
    : HttpClientDecorator(inner)
{
    public override HttpResponse Send(HttpRequest request)
    {
        var host = HttpUrl.GetHost(request.Url);
        if (!registry.TryGetIps(host, out var ips))
        {
            return Inner.Send(request);
        }

        var chosenIp = ips.FirstOrDefault(tracker.IsAlive);
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
}
