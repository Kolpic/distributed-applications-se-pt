using ProjectManagementAPI.ViewModels.Paging;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.ViewModels.Users
{
    /// <summary>
    /// Parameters for searching, filtering, and paging users
    /// </summary>
    public class UserSearchParameters : PagingParameters
    {
        /// <summary>
        /// Filter users by username (partial match)
        /// </summary>
        /// <example>john</example>
        [StringLength(50)]
        public string Username { get; set; }

        /// <summary>
        /// Filter users by first name (partial match)
        /// </summary>
        /// <example>John</example>
        [StringLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        /// Filter users by last name (partial match)
        /// </summary>
        /// <example>Doe</example>
        [StringLength(50)]
        public string LastName { get; set; }

        /// <summary>
        /// Filter users by admin status
        /// </summary>
        /// <example>true</example>
        public bool? IsAdmin { get; set; }
    }
}
