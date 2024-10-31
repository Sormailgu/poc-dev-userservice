using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Services;

namespace UserService.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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
            return Ok(_userService.GetAllUsers());
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
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
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
                var createdUser = await _userService.CreateUserAsync(user);
                return Ok(createdUser);
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

            try
            {
                _userService.UpdateUser(id, updatedUser);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error updating user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the user.");
            }
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
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
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
            try
            {
                var user = await _userService.DisableUserAsync(userId);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error disabling user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while disabling the user.");
            }
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
            _userService.StartBackgroundSync();
            return Ok("Background sync service started");
        }
    }
}