using Common.Entities;
using Common.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementAPI.ViewModels.Paging;
using ProjectManagementAPI.ViewModels.Projects;

namespace ProjectManagementAPI.Controllers
{
    /// <summary>
    /// Manages project-related operations including creating projects and managing their categories
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class ProjectsController : ControllerBase
    {
        /// <summary>
        /// Retrieves all projects with their associated details
        /// </summary>
        /// <remarks>
        /// This endpoint returns all projects with their owner, categories, and comments.
        /// Requires authentication.
        /// </remarks>
        /// <returns>A list of all projects with their details</returns>
        /// <response code="200">Returns the list of projects</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProjectsDTO>))]
        public IActionResult Get()
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();
            List<ProjectsDTO> res = context.Projects
                  .Include(p => p.Owner)
                  .Include(p => p.ProjectCategories)
                  .ThenInclude(pc => pc.Category)
            .Select(p => new ProjectsDTO
            {
              Id = p.Id,
              Name = p.Title,
              OwnerName = p.Owner.Username,
              Categories = p.ProjectCategories.Select(pc => pc.Category.Name).ToList(),
              Comments = p.Comments.Select(pc => pc.Content).ToList()
            })
            .ToList();

            context.Dispose();

            return Ok(res);
        }

        /// <summary>
        /// Searches for projects with filtering, paging, and sorting
        /// </summary>
        /// <remarks>
        /// Use this endpoint to search for projects with various filters and pagination.
        /// Requires authentication.
        /// 
        /// Example query: /api/Projects/search?title=web&amp;pageNumber=1&amp;pageSize=10&amp;sortBy=Title&amp;sortDirection=asc
        /// </remarks>
        /// <param name="parameters">Search, paging, and sorting parameters</param>
        /// <returns>A paged list of projects matching the search criteria</returns>
        /// <response code="200">Returns the filtered, paged list of projects</response>
        /// <response code="400">If the parameters are invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<ProjectsDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Search([FromQuery] ProjectSearchParameters parameters)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();

            var query = context.Projects
                .Include(p => p.Owner)
                .Include(p => p.ProjectCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.Comments)
                .AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrEmpty(parameters.Title))
            {
                query = query.Where(p => p.Title.Contains(parameters.Title));
            }

            if (!string.IsNullOrEmpty(parameters.Description))
            {
                query = query.Where(p => p.Description.Contains(parameters.Description));
            }

            if (parameters.OwnerId.HasValue)
            {
                query = query.Where(p => p.OwnerId == parameters.OwnerId.Value);
            }

            if (!string.IsNullOrEmpty(parameters.OwnerUsername))
            {
                query = query.Where(p => p.Owner.Username.Contains(parameters.OwnerUsername));
            }

            if (parameters.CategoryId.HasValue)
            {
                query = query.Where(p => p.ProjectCategories.Any(pc => pc.CategoryId == parameters.CategoryId.Value));
            }

            // Map to DTO before applying sort
            var dtoQuery = query.Select(p => new ProjectsDTO
            {
                Id = p.Id,
                Name = p.Title,
                OwnerName = p.Owner.Username,
                Categories = p.ProjectCategories.Select(pc => pc.Category.Name).ToList(),
                Comments = p.Comments.Select(pc => pc.Content).ToList()
            });

            // Apply sorting (with fallback to Id if sortBy doesn't exist)
            string sortBy = parameters.SortBy;
            if (sortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
            {
                sortBy = "Name"; // Map to the DTO property name
            }

            dtoQuery = dtoQuery.AsQueryable().ApplySort(sortBy, parameters.SortDirection);

            // Apply pagination and get result
            var result = dtoQuery.ToPagedResponse(parameters.PageNumber, parameters.PageSize);

            context.Dispose();

            return Ok(result);
        }

        /// <summary>
        /// Finds projects by title
        /// </summary>
        /// <remarks>
        /// Use this endpoint to find projects with a specific title (partial match).
        /// Requires authentication.
        /// </remarks>
        /// <param name="title">The title to search for</param>
        /// <returns>List of projects with matching titles</returns>
        /// <response code="200">Returns the matching projects</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("findByTitle/{title}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProjectsDTO>))]
        public IActionResult FindByTitle(string title)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();

            var projects = context.Projects
                .Include(p => p.Owner)
                .Include(p => p.ProjectCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.Comments)
                .Where(p => p.Title.Contains(title))
                .Select(p => new ProjectsDTO
                {
                    Id = p.Id,
                    Name = p.Title,
                    OwnerName = p.Owner.Username,
                    Categories = p.ProjectCategories.Select(pc => pc.Category.Name).ToList(),
                    Comments = p.Comments.Select(pc => pc.Content).ToList()
                })
                .ToList();

            context.Dispose();

            return Ok(projects);
        }


        /// <summary>
        /// Creates a new project
        /// </summary>
        /// <remarks>
        /// Use this endpoint to create a new project. The authenticated user will be set as the owner.
        /// Requires authentication.
        /// 
        /// Sample request:
        /// 
        ///     POST /api/Projects
        ///     {
        ///        "title": "New Web Application",
        ///        "description": "A web application for managing customer data"
        ///     }
        /// </remarks>
        /// <param name="item">The project data to create</param>
        /// <returns>The created project</returns>
        /// <response code="200">Returns the created project</response>
        /// <response code="400">If the project data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateProjectDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] CreateProjectDTO item)
      {
          int loggedUserId = Convert.ToInt32(User.FindFirst("LoggedUserId").Value);
          
          ProjectManagementDbContext context = new ProjectManagementDbContext();

          item.OwnerId = loggedUserId;

            Project newProject = new Project
            {
                OwnerId = loggedUserId,
                Title = item.Title,
                Description = item.Description
            };

            context.Projects.Add(newProject);
          context.SaveChanges();

          context.Dispose();

          return Ok(item);
      }

        /// <summary>
        /// Adds a category to a project
        /// </summary>
        /// <remarks>
        /// Use this endpoint to associate a category with a project.
        /// Requires authentication.
        /// </remarks>
        /// <param name="projectId">The ID of the project</param>
        /// <param name="categoryId">The ID of the category to add</param>
        /// <returns>Success response</returns>
        /// <response code="200">If the category was successfully added to the project</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the project or category doesn't exist</response>
        
        [HttpPost("{projectId}/categories/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AddCategoryToProject(int projectId, int categoryId)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();

            Project project = context.Projects.FirstOrDefault(p => p.Id == projectId);
            Category category = context.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (project == null || category == null)
                return NotFound();

            if (!context.ProjectCategories.Any(pc => pc.ProjectId == projectId && pc.CategoryId == categoryId))
            {
                context.ProjectCategories.Add(new ProjectCategory
                {
                    ProjectId = projectId,
                    CategoryId = categoryId
                });

                context.SaveChanges();
            }

            return Ok();
        }

        /// <summary>
        /// Removes a category from a project
        /// </summary>
        /// <remarks>
        /// Use this endpoint to remove a category association from a project.
        /// Requires authentication.
        /// </remarks>
        /// <param name="projectId">The ID of the project</param>
        /// <param name="categoryId">The ID of the category to remove</param>
        /// <returns>Success response</returns>
        /// <response code="200">If the category was successfully removed from the project</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the project-category association doesn't exist</response>

        [HttpDelete("{projectId}/categories/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult RemoveCategoryFromProject(int projectId, int categoryId)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();

            ProjectCategory projectCategory = context.ProjectCategories
                .FirstOrDefault(pc => pc.ProjectId == projectId && pc.CategoryId == categoryId);

            if (projectCategory == null) return NotFound();

            context.ProjectCategories.Remove(projectCategory);
            context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Gets all categories associated with a project
        /// </summary>
        /// <remarks>
        /// Use this endpoint to retrieve all categories linked to a specific project.
        /// Requires authentication.
        /// </remarks>
        /// <param name="projectId">The ID of the project</param>
        /// <returns>List of categories associated with the project</returns>
        /// <response code="200">Returns the list of categories for the project</response>
        /// <response code="401">If the user is not authenticated</response>
        
        [HttpGet("{projectId}/categories")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Category>))]
        public IActionResult GetCategoriesForProject(int projectId)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();

            var categories = context.ProjectCategories
                .Where(pc => pc.ProjectId == projectId)
                .Select(pc => pc.Category)
                .ToList();

            return Ok(categories);
        }
    }
}
