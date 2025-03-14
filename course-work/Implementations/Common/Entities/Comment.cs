using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    /// <summary>
    /// Represents a comment on a project
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// The unique identifier for the comment
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The content of the comment
        /// </summary>
        [Required]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment content must be between 1 and 1000 characters")]
        public string Content { get; set; }

        /// <summary>
        /// The ID of the project this comment is associated with
        /// </summary>
        [Required]
        public int ProjectId { get; set; }

        /// <summary>
        /// The ID of the user who created this comment
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// The date and time when the comment was created
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The project this comment is associated with
        /// </summary>
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }

        /// <summary>
        /// The user who created this comment
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
