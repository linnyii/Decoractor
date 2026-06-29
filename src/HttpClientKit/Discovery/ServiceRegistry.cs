namespace HttpClientKit.Discovery;

public sealed class ServiceRegistry
{
    private readonly Dictionary<string, IReadOnlyList<string>> _hostToIps = new();

    private void Register(string host, params string[] ips) => _hostToIps[host] = ips;

    public bool TryGetIps(string host, out IReadOnlyList<string> ips)
        => _hostToIps.TryGetValue(host, out ips!);

    public static ServiceRegistry FromConfig(string text)
    {
        var registry = new ServiceRegistry();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var line in lines)
        {
            var separator = line.IndexOf(':');
            if (separator < 0)
            {
                continue;
            }

            var host = line[..separator].Trim();
            var ips = line[(separator + 1)..]
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            registry.Register(host, ips);
        }

        return registry;
    }
}
