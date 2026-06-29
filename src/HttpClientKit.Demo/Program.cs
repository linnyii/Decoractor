using HttpClientKit;
using HttpClientKit.Decorators;
using HttpClientKit.Discovery;
using HttpClientKit.Exceptions;


static bool SuccessDecider(HttpRequest request) => !request.Url.Contains("35.0.0.1");

static (ServiceRegistry registry, IpHealthTracker tracker) NewBackends()
    => (ServiceRegistry.FromConfig("waterballsa.tw: 35.0.0.1, 35.0.0.2, 35.0.0.3"), new IpHealthTracker());

Console.WriteLine("=== 組合 A：服務探索 → 負載平衡 → 黑名單（連發 4 次）===");
{
    var (registry, tracker) = NewBackends();
    IHttpClient client =
        new BlackListDecorator(
            new LoadBalancingDecorator(
                new ServiceDiscoveryDecorator(
                    new FakeHttpClient(SuccessDecider), registry, tracker),
                registry, tracker),
            ["evil.tw"]);

    for (var i = 1; i <= 4; i++)
    {
        TrySend(client, "http://waterballsa.tw/mail", $"第 {i} 次");
    }
}

static void TrySend(IHttpClient client, string url, string label)
{
    try
    {
        var response = client.Send(new HttpRequest(url));
        Console.WriteLine($"  {label} → 成功，實際送達 {response.Url}");
    }
    catch (BlackListedHostException ex)
    {
        Console.WriteLine($"  {label} → 中止（黑名單）：{ex.Message}");
    }
    catch (HttpRequestFailedException ex)
    {
        Console.WriteLine($"  {label} → 失敗：{ex.Message}");
    }
}
