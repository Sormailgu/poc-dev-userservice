using UserService.Models;
using FinancialService.Client;
using ConfigureService.Client;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private static readonly List<User> Users = new List<User>
        {
            new User { Id = 1, Name = "John Doe", Email = "john.doe@example.com", DateOfBirth = new DateTime(1990, 1, 1), IsActive = true },
            new User { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com", DateOfBirth = new DateTime(1985, 5, 15), IsActive = true }
        };

        private readonly FinancialServiceClient _financialServiceClient;
        private readonly ConfigureServiceClient _configureServiceClient;
        private readonly BackgroundSyncService _backgroundSyncService;

        public UserService(FinancialServiceClient financialServiceClient,
            ConfigureServiceClient configureServiceClient,
            BackgroundSyncService backgroundSyncService)
        {
            _financialServiceClient = financialServiceClient;
            _configureServiceClient = configureServiceClient;
            _backgroundSyncService = backgroundSyncService;
        }

        public List<User> GetAllUsers()
        {
            return Users;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return null;
            }

            var response = await _financialServiceClient.AccountBalance[id].GetAsync();
            var accountTypeResponse = await _configureServiceClient.Api.AccountType.GetAccountTypeList.GetAsync();
            var accountType = accountTypeResponse.FirstOrDefault();

            user.UserWithBalance = new UserWithBalance
            {
                AccountId = (Guid)response.AccountId,
                Balance = (decimal)response.Balance,
                Currency = "$",
                LastUpdated = response.LastUpdated.HasValue ? response.LastUpdated.Value.DateTime : default(DateTime),
                Status = "Active"
            };

            user.UserAccountType = new UserAccountType
            {
                Name = accountType.Name,
                Description = accountType.Description,
                Id = accountType.Id,
            };

            return user;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Id = Users.Max(u => u.Id) + 1;
            Users.Add(user);

            // Initialize user balance
            // var initialBalance = new UserBalance
            // {
            //     UserId = user.Id,
            //     Balance = 0, // Set initial balance to 0 or any default value
            //     LastUpdated = DateTime.UtcNow
            // };

            //await _financialServiceClient.AccountBalance.PostAsync(initialBalance);

            return user;
        }

        public void UpdateUser(int id, User updatedUser)
        {
            if (updatedUser == null)
            {
                throw new ArgumentNullException(nameof(updatedUser));
            }

            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.DateOfBirth = updatedUser.DateOfBirth;
            user.IsActive = updatedUser.IsActive;
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Remove the user's balance
            //await _financialServiceClient.AccountBalance[id].DeleteAsync();

            // Remove the user
            Users.Remove(user);
        }

        public async Task<User> DisableUserAsync(int userId)
        {
            var user = Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.IsActive = false; // Assuming IsActive is a property that indicates if the user is active

            var disableResponse = await _financialServiceClient.AccountBalance[userId].Disable.PutAsync();
            //var response = await _financialServiceClient.AccountBalance[userId].GetAsync();
            user.UserWithBalance = null;

            return user;
        }

        public void StartBackgroundSync()
        {
            _backgroundSyncService.Start();
        }
    }
}