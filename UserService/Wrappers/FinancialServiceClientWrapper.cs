using FinancialService.Client;
using FinancialService.Client.Models;

public class FinancialServiceClientWrapper : IFinancialServiceClientWrapper
{
    private readonly FinancialServiceClient _client;

    public FinancialServiceClientWrapper(FinancialServiceClient client)
    {
        _client = client;
    }

    public async Task<AccountBalanceResponseDto> GetAccountBalanceAsync(int userId)
    {
        return await _client.AccountBalance[userId].GetAsync();
    }

    // public async Task<AccountBalanceResponseDto> UpdateBalanceAsync(int userId, UpdateBalanceRequestDto requestDto) {
    //     return await _client.AccountBalance[userId].UpdateBalanceAsync(requestDto);

    // }

    // public async Task<AccountBalanceResponseDto> CreateAccountBalanceAsync(int userId, CreateAccountBalanceRequestDto requestDto) {
    //     return await _client.AccountBalanceRequestBuilder.PostAsync(requestDto);
    // }

    public async Task DisableAccountAsync(int userId)
    {
        await _client.AccountBalance[userId].Disable.PutAsync();
    }
}
