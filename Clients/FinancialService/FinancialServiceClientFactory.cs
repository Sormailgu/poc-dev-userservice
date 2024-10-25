using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace FinancialService.Client
{
  public class FinancialServiceClientFactory
  {
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly HttpClient _httpClient;

    public FinancialServiceClientFactory(HttpClient httpClient)
    {
      _authenticationProvider = new AnonymousAuthenticationProvider();
      _httpClient = httpClient;
    }

    public FinancialServiceClient GetClient()
    {
      return new FinancialServiceClient(new HttpClientRequestAdapter(_authenticationProvider, httpClient: _httpClient));
    }
  }
}