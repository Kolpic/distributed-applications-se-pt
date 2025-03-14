using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.ViewModels.Comments
{
    /// <summary>
    /// Data transfer object for creating a new comment
    /// </summary>
    public class CreateCommentDTO
    {
        /// <summary>
        /// The content of the comment
        /// </summary>
        /// <example>This is a great project idea!</example>
        [Required(ErrorMessage = "Comment content is required!")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment content must be between 1 and 1000 characters")]
        public string Content { get; set; }

        /// <summary>
        /// The ID of the project this comment is associated with
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "Project ID is required!")]
        public int ProjectId { get; set; }
    }
}
