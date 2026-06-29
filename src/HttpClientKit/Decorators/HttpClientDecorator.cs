namespace HttpClientKit.Decorators;

public abstract class HttpClientDecorator(IHttpClient inner) : IHttpClient
{
    protected readonly IHttpClient Inner = inner;

    public abstract HttpResponse Send(HttpRequest request);
}
