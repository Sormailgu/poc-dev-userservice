using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace ConfigureService.Client
{
  public class ConfigureServiceClientFactory
  {
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly HttpClient _httpClient;

    public ConfigureServiceClientFactory(HttpClient httpClient)
    {
      _authenticationProvider = new AnonymousAuthenticationProvider();
      _httpClient = httpClient;
    }

    public ConfigureServiceClient GetClient()
    {
      return new ConfigureServiceClient(new HttpClientRequestAdapter(_authenticationProvider, httpClient: _httpClient));
    }
  }
}

