using System;
using System.Threading;
using System.Threading.Tasks;
using FinancialService.Client;

namespace UserService.Services
{

    public class BackgroundSyncService
    {
        private readonly FinancialServiceClient _financialServiceClient;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly TimeSpan _interval;

        public BackgroundSyncService(FinancialServiceClient financialServiceClient, TimeSpan interval)
        {
            _financialServiceClient = financialServiceClient;
            _cancellationTokenSource = new CancellationTokenSource();
            _interval = interval;
        }

        public void Start()
        {
            Task.Run(async () => await RunAsync(_cancellationTokenSource.Token));
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await GetUserWithBalance();
                await Task.Delay(_interval, cancellationToken);
            }
        }

        private async Task GetUserWithBalance()
        {
            try
            {
                var response = await _financialServiceClient.AccountBalance[1].GetAsync();
                // Print the response
                Console.WriteLine($"[{DateTime.Now}] Background Sync: Performing sync operation...");
                Console.WriteLine(response.AccountId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }
    }
}
