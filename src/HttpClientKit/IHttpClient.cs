namespace HttpClientKit;

public interface IHttpClient
{
    HttpResponse Send(HttpRequest request);
}
