using HttpClientKit.Exceptions;

namespace HttpClientKit;

public sealed class FakeHttpClient : IHttpClient
{
    private readonly Func<HttpRequest, bool> _successDecider;

    public FakeHttpClient(Func<HttpRequest, bool>? successDecider = null)
    {
        var random = new Random();
        _successDecider = successDecider ?? (_ => random.Next(2) == 0);
    }

    public HttpResponse Send(HttpRequest request)
    {
        if (_successDecider(request))
        {
            Console.WriteLine($"[SUCCESS] {request.Url}");
            return new HttpResponse(request.Url, true);
        }

        Console.WriteLine($"[FAILED] {request.Url}");
        throw new HttpRequestFailedException(request.Url);
    }
}
