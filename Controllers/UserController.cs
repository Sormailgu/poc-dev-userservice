using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FinancialService.Client;
using ConfigureService.Client;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Abstractions.Authentication;
using UserService.Services;

namespace UserService.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly FinancialServiceClient _financialServiceClient;
        private readonly ConfigureServiceClient _configureServiceClient;
        private readonly BackgroundSyncService _backgroundSyncService;

        private static readonly List<User> Users = new List<User>
    {
        new User { Id = 1, Name = "John Doe", Email = "john.doe@example.com", DateOfBirth = new DateTime(1990, 1, 1), IsActive = true },
        new User { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com", DateOfBirth = new DateTime(1985, 5, 15), IsActive = true }
    };

        public UserController(FinancialServiceClient financialServiceClient,
        ConfigureServiceClient configureServiceClient,
        BackgroundSyncService backgroundSyncService)
        {
            _financialServiceClient = financialServiceClient;
            _configureServiceClient = configureServiceClient;
            _backgroundSyncService = backgroundSyncService;
        }

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A list of all users.</returns>
        /// <response code="200">Returns the list of users</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(Users);
        }

        /// <summary>
        /// Retrieves a specific user by id.
        /// </summary>
        /// <param name="id">The id of the user to retrieve.</param>
        /// <returns>The user with the specified id.</returns>
        /// <response code="200">Returns the user with the specified id</response>
        /// <response code="404">If the user is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var response = await _financialServiceClient.AccountBalance[id].GetAsync();

            var accountTypeResponse = await _configureServiceClient.Api.AccountType.GetAccountTypeList.GetAsync();

            var accountType = accountTypeResponse.FirstOrDefault();

            var result = new User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                IsActive = user.IsActive,
                UserWithBalance = new UserWithBalance
                {
                    AccountId = (Guid)response.AccountId,
                    Balance = (decimal)response.Balance,
                    Currency = "$",
                    LastUpdated = response.LastUpdated.HasValue ? response.LastUpdated.Value.DateTime : default(DateTime),
                    Status = "Active"
                },
                UserAccountType = new UserAccountType
                {
                    Name = accountType.Name,
                    Description = accountType.Description,
                    Id = accountType.Id,
                }
            };

            return Ok(result);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <returns>The created user.</returns>
        /// <response code="201">Returns the newly created user</response>
        /// <response code="400">If the user is null</response>
        /// <response code="500">If an error occurs</response>
        [HttpPost]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            try
            {
                // Create the user
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

                return Ok(user);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error creating user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the user.");
            }
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The id of the user to update.</param>
        /// <param name="updatedUser">The updated user information.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the user was successfully updated</response>
        /// <response code="400">If the user is null</response>
        /// <response code="404">If the user is not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateUser(int id, User updatedUser)
        {
            if (updatedUser == null)
            {
                return BadRequest();
            }

            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.DateOfBirth = updatedUser.DateOfBirth;
            user.IsActive = updatedUser.IsActive;
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific user.
        /// </summary>
        /// <param name="id">The id of the user to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the user was successfully deleted</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If an error occurs</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                // Remove the user's balance
                //await _financialServiceClient.AccountBalance[id].DeleteAsync();

                // Remove the user
                Users.Remove(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the user.");
            }
        }

        [HttpPost("disable/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<User>> DisableUser(int userId)
        {
            var user = Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = false; // Assuming IsActive is a property that indicates if the user is active

            var disableResponse = await _financialServiceClient.AccountBalance[userId].Disable.PutAsync();
            //var response = await _financialServiceClient.AccountBalance[userId].GetAsync();
            user.UserWithBalance = null;

            return Ok(user);
        }

        /// <summary>
        /// Triggers the background sync service to start.
        /// </summary>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the background sync service was successfully started</response>
        [HttpPost("start-background-sync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult StartBackgroundSync()
        {
            _backgroundSyncService.Start();
            return Ok("Background sync service started");
        }
    }
}

