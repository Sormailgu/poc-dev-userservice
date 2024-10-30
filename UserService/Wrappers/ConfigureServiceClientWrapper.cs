using ConfigureService.Client;
using ConfigureService.Client.Models;

public class ConfigureServiceClientWrapper : IConfigureServiceClientWrapper
{
    private readonly ConfigureServiceClient _client;

    public ConfigureServiceClientWrapper(ConfigureServiceClient client)
    {
        _client = client;
    }

    public Task<List<AccountTypeResponseDto>> GetAccountTypeListAsync()
    {
        return _client.Api.AccountType.GetAccountTypeList.GetAsync();
    }
}