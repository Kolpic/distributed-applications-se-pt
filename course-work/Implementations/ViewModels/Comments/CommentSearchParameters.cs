using ProjectManagementAPI.ViewModels.Paging;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.ViewModels.Comments
{
    /// <summary>
    /// Parameters for searching, filtering, and paging comments
    /// </summary>
    public class CommentSearchParameters : PagingParameters
    {
        /// <summary>
        /// Filter comments by content (partial match)
        /// </summary>
        /// <example>great</example>
        [StringLength(100)]
        public string Content { get; set; }

        /// <summary>
        /// Filter comments by user ID
        /// </summary>
        /// <example>1</example>
        public int? UserId { get; set; }

        /// <summary>
        /// Filter comments by username (partial match)
        /// </summary>
        /// <example>john</example>
        [StringLength(50)]
        public string Username { get; set; }

        /// <summary>
        /// Filter comments by project ID
        /// </summary>
        /// <example>2</example>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Filter comments by date range (from)
        /// </summary>
        /// <example>2023-01-01T00:00:00Z</example>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Filter comments by date range (to)
        /// </summary>
        /// <example>2023-12-31T23:59:59Z</example>
        public DateTime? ToDate { get; set; }
    }
}
