namespace HttpClientKit.Discovery;

public sealed class IpHealthTracker(Func<DateTime>? now = null)
{
    private static readonly TimeSpan InvalidDuration = TimeSpan.FromMinutes(10);

    private readonly Dictionary<string, DateTime> _expireAt = new();
    private readonly Func<DateTime> _now = now ?? (() => DateTime.UtcNow);

    public bool IsAlive(string ip)
    {
        if (!_expireAt.TryGetValue(ip, out var expireAt))
        {
            return true;
        }

        if (_now() >= expireAt)
        {
            _expireAt.Remove(ip);
            return true;
        }

        return false;
    }

    public void MarkFailed(string ip) => _expireAt[ip] = _now() + InvalidDuration;
}
