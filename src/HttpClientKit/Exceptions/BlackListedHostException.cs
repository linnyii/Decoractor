namespace HttpClientKit.Exceptions;

/// <summary>請求的 host 命中黑名單時拋出，藉此中止請求。</summary>
public sealed class BlackListedHostException : Exception
{
    public BlackListedHostException(string host)
        : base($"Host is black-listed: {host}")
    {
    }
}
