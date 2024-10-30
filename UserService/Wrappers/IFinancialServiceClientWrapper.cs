using FinancialService.Client;
using FinancialService.Client.Models;

public interface IFinancialServiceClientWrapper
{
    Task<AccountBalanceResponseDto> GetAccountBalanceAsync(int userId);
    Task<AccountBalanceResponseDto> UpdateBalanceAsync(int userId, UpdateBalanceRequestDto requestDto);
    Task<AccountBalanceResponseDto> CreateAccountBalanceAsync(int userId, CreateAccountBalanceRequestDto requestDto);
    Task DisableAccountAsync(int userId);
}
