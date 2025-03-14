using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.ViewModels.Comments
{
    /// <summary>
    /// Data transfer object for displaying comment details
    /// </summary>
    public class CommentDTO
    {
        /// <summary>
        /// The unique identifier of the comment
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// The content of the comment
        /// </summary>
        /// <example>This is a great project idea!</example>
        
        [StringLength(1000)]
        public string Content { get; set; }

        /// <summary>
        /// The date and time when the comment was created
        /// </summary>
        /// <example>2023-03-01T12:00:00Z</example>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The ID of the project this comment is associated with
        /// </summary>
        /// <example>1</example>
        public int ProjectId { get; set; }

        /// <summary>
        /// The title of the project this comment is associated with
        /// </summary>
        /// <example>E-commerce Platform</example>
        [StringLength(100)]
        public string ProjectTitle { get; set; }

        /// <summary>
        /// The ID of the user who created this comment
        /// </summary>
        /// <example>1</example>
        public int UserId { get; set; }

        /// <summary>
        /// The username of the user who created this comment
        /// </summary>
        /// <example>johndoe</example>
        [StringLength(50)]
        public string Username { get; set; }
    }
}
