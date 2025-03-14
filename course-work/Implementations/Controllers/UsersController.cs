using Common.Entities;
using Common.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.ViewModels.Paging;
using ProjectManagementAPI.ViewModels.Users;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectManagementAPI.Controllers
{
    /// <summary>
    /// Manages user-related operations such as creating, reading, updating, and deleting users
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// Retrieves all users in the system
        /// </summary>
        /// <remarks>
        /// This endpoint returns a list of all registered users.
        /// Requires authentication.
        /// </remarks>
        /// <returns>A list of all users</returns>
        /// <response code="200">Returns the list of users</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
        public IActionResult Get()
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();
            List<User> res = context.Users.ToList();
            
            context.Dispose();

            return Ok(res);
        }

        /// <summary>
        /// Searches for users with filtering, paging, and sorting
        /// </summary>
        /// <remarks>
        /// Use this endpoint to search for users with various filters and pagination.
        /// Requires authentication.
        /// 
        /// Example query: /api/Users/search?username=john&amp;pageNumber=1&amp;pageSize=10&amp;sortBy=Username&amp;sortDirection=asc
        /// </remarks>
        /// <param name="parameters">Search, paging, and sorting parameters</param>
        /// <returns>A paged list of users matching the search criteria</returns>
        /// <response code="200">Returns the filtered, paged list of users</response>
        /// <response code="400">If the parameters are invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<User>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Search([FromQuery] UserSearchParameters parameters)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();

            var query = context.Users.AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrEmpty(parameters.Username))
            {
                query = query.Where(u => u.Username.Contains(parameters.Username));
            }

            if (!string.IsNullOrEmpty(parameters.FirstName))
            {
                query = query.Where(u => u.FirstName.Contains(parameters.FirstName));
            }

            if (!string.IsNullOrEmpty(parameters.LastName))
            {
                query = query.Where(u => u.LastName.Contains(parameters.LastName));
            }

            if (parameters.IsAdmin.HasValue)
            {
                query = query.Where(u => u.IsAdmin == parameters.IsAdmin.Value);
            }

            // Apply sorting
            query = query.ApplySort(parameters.SortBy, parameters.SortDirection);

            // Apply pagination and get result
            var result = query.ToPagedResponse(parameters.PageNumber, parameters.PageSize);

            context.Dispose();

            return Ok(result);
        }

        /// <summary>
        /// Finds users by username
        /// </summary>
        /// <remarks>
        /// Use this endpoint to find users with a specific username (exact match).
        /// Requires authentication.
        /// </remarks>
        /// <param name="username">The username to search for</param>
        /// <returns>The user with the specified username</returns>
        /// <response code="200">Returns the requested user</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If no user with the specified username exists</response>
        [HttpGet("findByUsername/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult FindByUsername(string username)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();
            User user = context.Users
                                .Where(i => i.Username == username)
                                .FirstOrDefault();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Retrieves a specific user by ID
        /// </summary>
        /// <remarks>
        /// Use this endpoint to get detailed information about a specific user.
        /// Requires authentication.
        /// </remarks>
        /// <param name="id">The unique identifier of the user</param>
        /// <returns>The user with the specified ID</returns>
        /// <response code="200">Returns the requested user</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the user doesn't exist</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(int id)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();
            User user = context.Users
                                .Where(i => i.Id == id)
                                .FirstOrDefault();

            return Ok(user);
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <remarks>
        /// Use this endpoint to register a new user in the system.
        /// Requires authentication.
        /// 
        /// Sample request:
        /// 
        ///     POST /api/Users
        ///     {
        ///        "username": "johndoe",
        ///        "password": "securepassword",
        ///        "firstName": "John",
        ///        "lastName": "Doe",
        ///        "isAdmin": false
        ///     }
        /// </remarks>
        /// <param name="item">The user data to create</param>
        /// <returns>The created user</returns>
        /// <response code="200">Returns the created user</response>
        /// <response code="400">If the user data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateUserDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] CreateUserDTO item)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();

            User user = new User
            {
                Username = item.Username,
                Password = item.Password,
                FirstName = item.FirstName,
                LastName = item.LastName,
                IsAdmin = item.IsAdmin,
            };

            context.Users.Add(user);
            context.SaveChanges();

            context.Dispose();

            return Ok(item);
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <remarks>
        /// Use this endpoint to update a user's information.
        /// Requires authentication.
        /// 
        /// Sample request:
        /// 
        ///     PUT /api/Users
        ///     {
        ///        "id": 1,
        ///        "username": "johndoe",
        ///        "password": "newsecurepassword",
        ///        "firstName": "John",
        ///        "lastName": "Doe"
        ///     }
        /// </remarks>
        /// <param name="item">The user data to update</param>
        /// <returns>The updated user</returns>
        /// <response code="200">Returns the updated user</response>
        /// <response code="400">If the user data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the user doesn't exist</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put([FromBody] EditUserDTO item)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();
            User user = context.Users
                                .Where(i => i.Id == item.Id)
                                .FirstOrDefault();

            user.Username = item.Username;
            user.Password = item.Password;
            user.FirstName = item.FirstName;
            user.LastName = item.LastName;

            context.Users.Update(user);
            context.SaveChanges();

            context.Dispose();

            return Ok(user);
        }

        /// <summary>
        /// Deletes a user by ID
        /// </summary>
        /// <remarks>
        /// Use this endpoint to remove a user from the system.
        /// Requires authentication.
        /// </remarks>
        /// <param name="id">The unique identifier of the user to delete</param>
        /// <returns>The deleted user</returns>
        /// <response code="200">Returns the deleted user</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the user doesn't exist</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();
            User item = context.Users
                                .Where(i => i.Id == id)
                                .FirstOrDefault();
            if (item != null)
            {
                context.Users.Remove(item);
                context.SaveChanges();

                context.Dispose();

                return Ok(item);
            }

            context.Dispose();

            return NotFound();
        }
    }
}
