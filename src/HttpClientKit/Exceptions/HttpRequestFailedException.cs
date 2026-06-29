namespace HttpClientKit.Exceptions;

/// <summary>底層 HTTP Client 送出請求失敗時拋出（服務探索會據此把 IP 標記失效）。</summary>
public sealed class HttpRequestFailedException : Exception
{
    public HttpRequestFailedException(string url)
        : base($"HTTP request failed: {url}")
    {
    }
}
