using ConfigureService.Client;
using ConfigureService.Client.Models;

public interface IConfigureServiceClientWrapper
{
    Task<List<AccountTypeResponseDto>> GetAccountTypeListAsync();
}