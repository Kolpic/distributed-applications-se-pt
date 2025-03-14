using ProjectManagementAPI.ViewModels.Paging;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.ViewModels.Projects
{
    /// <summary>
    /// Parameters for searching, filtering, and paging projects
    /// </summary>
    public class ProjectSearchParameters : PagingParameters
    {
        /// <summary>
        /// Filter projects by title (partial match)
        /// </summary>
        /// <example>Web</example>
        [StringLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// Filter projects by description (partial match)
        /// </summary>
        /// <example>management</example>
        [StringLength(100)]
        public string Description { get; set; }

        /// <summary>
        /// Filter projects by owner ID
        /// </summary>
        /// <example>1</example>
        public int? OwnerId { get; set; }

        /// <summary>
        /// Filter projects by owner username (partial match)
        /// </summary>
        /// <example>john</example>
        [StringLength(50)]
        public string OwnerUsername { get; set; }

        /// <summary>
        /// Filter projects by category ID
        /// </summary>
        /// <example>3</example>
        public int? CategoryId { get; set; }
    }
}
