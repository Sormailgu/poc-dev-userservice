using FinancialService.Client;
using ConfigureService.Client;
using System.Net.Http;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace UserService.Library.Clients
{
    public interface IClientFactory
    {
        FinancialServiceClient CreateFinancialServiceClient();
        ConfigureServiceClient CreateConfigureServiceClient();
    }

    public class ClientFactory : IClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public FinancialServiceClient CreateFinancialServiceClient()
        {
            var client = _httpClientFactory.CreateClient();
            return new FinancialServiceClient(new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), client));
        }

        public ConfigureServiceClient CreateConfigureServiceClient()
        {
            var client = _httpClientFactory.CreateClient();
            return new ConfigureServiceClient(new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), client));
        }
    }
}