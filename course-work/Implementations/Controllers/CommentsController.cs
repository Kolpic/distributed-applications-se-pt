using Common.Entities;
using Common.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementAPI.ViewModels.Comments;
using ProjectManagementAPI.ViewModels.Paging;

namespace ProjectManagementAPI.Controllers
{
    /// <summary>
    /// Manages comment-related operations including creating, reading, updating, and deleting comments
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class CommentsController : Controller
    {
        /// <summary>
        /// Retrieves all comments with their associated details
        /// </summary>
        /// <remarks>
        /// This endpoint returns all comments with their project and user information.
        /// Requires authentication.
        /// </remarks>
        /// <returns>List of all comments with their details</returns>
        /// <response code="200">Returns the list of comments</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommentDTO>))]
        public IActionResult Get()
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();
            List<CommentDTO> comments = context.Comments
                .Include(c => c.Project)
                .Include(c => c.User)
                .Select(c => new CommentDTO
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    ProjectId = c.Project.Id,
                    ProjectTitle = c.Project.Title,
                    UserId = c.User.Id,
                    Username = c.User.Username
                })       
                .ToList();

             return Ok(comments);
        }

        /// <summary>
        /// Retrieves a specific comment by ID
        /// </summary>
        /// <remarks>
        /// Use this endpoint to get detailed information about a specific comment.
        /// Requires authentication.
        /// </remarks>
        /// <param name="id">The unique identifier of the comment</param>
        /// <returns>The comment with the specified ID</returns>
        /// <response code="200">Returns the requested comment</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the comment doesn't exist</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommentDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(int id)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();
            CommentDTO comment = context.Comments
                .Include(c => c.Project)
                .Include(c => c.User)
                .Where(c => c.Id == id)
                .Select(c => new CommentDTO
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    ProjectId = c.Project.Id,
                    ProjectTitle = c.Project.Title,
                    UserId = c.User.Id,
                    Username = c.User.Username
                })
                .FirstOrDefault();

            if (comment == null) return NotFound();

            return Ok(comment);
        }

        /// <summary>
        /// Searches for comments with filtering, paging, and sorting
        /// </summary>
        /// <remarks>
        /// Use this endpoint to search for comments with various filters and pagination.
        /// Requires authentication.
        /// 
        /// Example query: /api/Comments/search?content=great&amp;pageNumber=1&amp;pageSize=10&amp;sortBy=CreatedAt&amp;sortDirection=desc
        /// </remarks>
        /// <param name="parameters">Search, paging, and sorting parameters</param>
        /// <returns>A paged list of comments matching the search criteria</returns>
        /// <response code="200">Returns the filtered, paged list of comments</response>
        /// <response code="400">If the parameters are invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<CommentDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Search([FromQuery] CommentSearchParameters parameters)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();

            var query = context.Comments
                .Include(c => c.Project)
                .Include(c => c.User)
                .AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrEmpty(parameters.Content))
            {
                query = query.Where(c => c.Content.Contains(parameters.Content));
            }

            if (parameters.UserId.HasValue)
            {
                query = query.Where(c => c.UserId == parameters.UserId.Value);
            }

            if (!string.IsNullOrEmpty(parameters.Username))
            {
                query = query.Where(c => c.User.Username.Contains(parameters.Username));
            }

            if (parameters.ProjectId.HasValue)
            {
                query = query.Where(c => c.ProjectId == parameters.ProjectId.Value);
            }

            if (parameters.FromDate.HasValue)
            {
                query = query.Where(c => c.CreatedAt >= parameters.FromDate.Value);
            }

            if (parameters.ToDate.HasValue)
            {
                query = query.Where(c => c.CreatedAt <= parameters.ToDate.Value);
            }

            // Map to DTO before applying sort
            var dtoQuery = query.Select(c => new CommentDTO
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                ProjectId = c.ProjectId,
                ProjectTitle = c.Project.Title,
                UserId = c.UserId,
                Username = c.User.Username
            });

            // Apply sorting
            dtoQuery = dtoQuery.AsQueryable().ApplySort(parameters.SortBy, parameters.SortDirection);

            // Apply pagination and get result
            var result = dtoQuery.ToPagedResponse(parameters.PageNumber, parameters.PageSize);

            context.Dispose();

            return Ok(result);
        }

        /// <summary>
        /// Finds comments by content
        /// </summary>
        /// <remarks>
        /// Use this endpoint to find comments containing specific text.
        /// Requires authentication.
        /// </remarks>
        /// <param name="content">The text to search for in comments</param>
        /// <returns>List of comments containing the specified text</returns>
        /// <response code="200">Returns the matching comments</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("findByContent/{content}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommentDTO>))]
        public IActionResult FindByContent(string content)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();

            var comments = context.Comments
                .Include(c => c.Project)
                .Include(c => c.User)
                .Where(c => c.Content.Contains(content))
                .Select(c => new CommentDTO
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    ProjectId = c.Project.Id,
                    ProjectTitle = c.Project.Title,
                    UserId = c.User.Id,
                    Username = c.User.Username
                })
                .ToList();

            context.Dispose();

            return Ok(comments);
        }

        /// <summary>
        /// Creates a new comment
        /// </summary>
        /// <remarks>
        /// Use this endpoint to add a comment to a project. The authenticated user will be set as the comment author.
        /// Requires authentication.
        /// 
        /// Sample request:
        /// 
        ///     POST /api/Comments
        ///     {
        ///        "content": "This is a great project idea!",
        ///        "projectId": 1
        ///     }
        /// </remarks>
        /// <param name="createDto">The comment data to create</param>
        /// <returns>The created comment</returns>
        /// <response code="200">Returns the created comment</response>
        /// <response code="400">If the comment data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommentDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] CreateCommentDTO createDto)
        {
            int loggedUserId = Convert.ToInt32(User.FindFirst("LoggedUserId").Value);

            ProjectManagementDbContext context = new ProjectManagementDbContext();
            Comment comment = new Comment
            {
                Content = createDto.Content,
                ProjectId = createDto.ProjectId,
                UserId = loggedUserId,
                CreatedAt = DateTime.UtcNow
            };

            context.Comments.Add(comment);
            context.SaveChanges();

            CommentDTO commentDto = new CommentDTO
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                ProjectId = comment.ProjectId,
                ProjectTitle = context.Projects.Find(comment.ProjectId).Title,
                UserId = comment.UserId,
                Username = context.Users.Find(comment.UserId).Username
            };

            return Ok(commentDto);
        }

        /// <summary>
        /// Updates an existing comment
        /// </summary>
        /// <remarks>
        /// Use this endpoint to update a comment's content. Only the comment author can update the comment.
        /// Requires authentication.
        /// 
        /// Sample request:
        /// 
        ///     PUT /api/Comments/1
        ///     {
        ///        "content": "Updated comment text"
        ///     }
        /// </remarks>
        /// <param name="id">The unique identifier of the comment to update</param>
        /// <param name="updateDto">The updated comment data</param>
        /// <returns>The updated comment</returns>
        /// <response code="200">Returns the updated comment</response>
        /// <response code="400">If the comment data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not the author of the comment</response>
        /// <response code="404">If the comment doesn't exist</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommentDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(int id, [FromBody] UpdateCommentDTO updateDto)
        {
            int loggedUserId = Convert.ToInt32(User.FindFirst("LoggedUserId").Value);

            ProjectManagementDbContext context = new ProjectManagementDbContext();
            Comment existingComment = context.Comments.FirstOrDefault(c => c.Id == id);

            if (existingComment == null) return NotFound();

            if (existingComment.UserId != loggedUserId) return Forbid();

            existingComment.Content = updateDto.Content;
            context.SaveChanges();

            CommentDTO commentDto = new CommentDTO
            {
                Id = existingComment.Id,
                Content = existingComment.Content,
                CreatedAt = existingComment.CreatedAt,
                ProjectId = existingComment.ProjectId,
                ProjectTitle = context.Projects.Find(existingComment.ProjectId).Title,
                UserId = existingComment.UserId,
                Username = context.Users.Find(existingComment.UserId).Username
            };

            return Ok(commentDto);
        }

        /// <summary>
        /// Deletes a comment by ID
        /// </summary>
        /// <remarks>
        /// Use this endpoint to remove a comment. Only the comment author can delete the comment.
        /// Requires authentication.
        /// </remarks>
        /// <param name="id">The unique identifier of the comment to delete</param>
        /// <returns>Success response</returns>
        /// <response code="200">If the comment was successfully deleted</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not the author of the comment</response>
        /// <response code="404">If the comment doesn't exist</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            int loggedUserId = Convert.ToInt32(User.FindFirst("LoggedUserId").Value);

            ProjectManagementDbContext context = new ProjectManagementDbContext();
            Comment comment = context.Comments.FirstOrDefault(c => c.Id == id);

            if (comment == null) return NotFound();

            if (comment.UserId != loggedUserId) return Forbid();

            context.Comments.Remove(comment);
            context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Retrieves all comments for a specific project
        /// </summary>
        /// <remarks>
        /// Use this endpoint to get all comments associated with a particular project.
        /// Requires authentication.
        /// </remarks>
        /// <param name="projectId">The ID of the project</param>
        /// <returns>List of comments for the specified project</returns>
        /// <response code="200">Returns the list of comments for the project</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet("project/{projectId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommentDTO>))]
        public IActionResult GetCommentsByProject(int projectId)
        {
            ProjectManagementDbContext context = new ProjectManagementDbContext();
            List<CommentDTO> comments = context.Comments
                .Where(c => c.ProjectId == projectId)
                .Include(c => c.User)
                .Select(c => new CommentDTO
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.User.Id,
                    Username = c.User.Username,
                    ProjectId = c.ProjectId,
                    ProjectTitle = c.Project.Title
                })
                .ToList();

            return Ok(comments);
        }

    }
}
