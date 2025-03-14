using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.ViewModels.Comments
{
    /// <summary>
    /// Data transfer object for updating an existing comment
    /// </summary>
    public class UpdateCommentDTO
    {
        /// <summary>
        /// The updated content of the comment
        /// </summary>
        /// <example>Updated comment: This is an excellent project idea!</example>
        [Required(ErrorMessage = "Comment content is required!")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment content must be between 1 and 1000 characters")]
        public string Content { get; set; }
    }
}
