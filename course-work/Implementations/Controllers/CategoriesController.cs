using Common.Entities;
using Common.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManagementAPI.Controllers
{
    /// <summary>
    /// Manages category-related operations such as creating, reading, updating, and deleting categories
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class CategoriesController : ControllerBase
    {
        /// <summary>
        /// Retrieves all categories
        /// </summary>
        /// <remarks>
        /// This endpoint returns a list of all available categories.
        /// Requires authentication.
        /// </remarks>
        /// <returns>A list of all categories</returns>
        /// <response code="200">Returns the list of categories</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Category>))]
        public IActionResult Get()
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();
            List<Category> categories = context.Categories.ToList();
            
            context.Dispose();

            return Ok(categories);
        }

        /// <summary>
        /// Retrieves a specific category by ID
        /// </summary>
        /// <remarks>
        /// Use this endpoint to get detailed information about a specific category.
        /// Requires authentication.
        /// </remarks>
        /// <param name="id">The unique identifier of the category</param>
        /// <returns>The category with the specified ID</returns>
        /// <response code="200">Returns the requested category</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the category doesn't exist</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(int id)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();
            Category category = context.Categories
                                        .Where(i => i.Id == id)
                                        .FirstOrDefault();
            
            if (category == null) return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <remarks>
        /// Use this endpoint to create a new project category.
        /// Requires authentication.
        /// 
        /// Sample request:
        /// 
        ///     POST /api/Categories
        ///     {
        ///        "name": "Frontend",
        ///        "description": "Frontend development technologies and frameworks"
        ///     }
        /// </remarks>
        /// <param name="category">The category data to create</param>
        /// <returns>The created category</returns>
        /// <response code="200">Returns the created category</response>
        /// <response code="400">If the category data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] Category category)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();
            context.Categories.Add(category);
            context.SaveChanges();

            context.Dispose();

            return Ok(category);
        }

        /// <summary>
        /// Updates an existing category
        /// </summary>
        /// <remarks>
        /// Use this endpoint to update a category's information.
        /// Requires authentication.
        /// 
        /// Sample request:
        /// 
        ///     PUT /api/Categories
        ///     {
        ///        "id": 1,
        ///        "name": "Frontend Development",
        ///        "description": "Frontend development technologies, frameworks and libraries"
        ///     }
        /// </remarks>
        /// <param name="category">The category data to update</param>
        /// <returns>The updated category</returns>
        /// <response code="200">Returns the updated category</response>
        /// <response code="400">If the category data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the category doesn't exist</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put([FromBody] Category category)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();
            Category existingCategory = context.Categories
                                                .Where(i => i.Id == category.Id)
                                                .FirstOrDefault();
            if (existingCategory == null) return NotFound();

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            context.Categories.Update(existingCategory);
            context.SaveChanges();

            context.Dispose();

            return Ok(existingCategory);
        }

        /// <summary>
        /// Deletes a category by ID
        /// </summary>
        /// <remarks>
        /// Use this endpoint to remove a category from the system.
        /// Requires authentication.
        /// </remarks>
        /// <param name="id">The unique identifier of the category to delete</param>
        /// <returns>The deleted category</returns>
        /// <response code="200">Returns the deleted category</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the category doesn't exist</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();
            Category category = context.Categories
                                        .Where(i => i.Id == id)
                                        .FirstOrDefault();
            
            if (category == null) return NotFound();

            context.Categories.Remove(category);
            context.SaveChanges();

            context.Dispose();

            return Ok(category);
        }
    }
}
