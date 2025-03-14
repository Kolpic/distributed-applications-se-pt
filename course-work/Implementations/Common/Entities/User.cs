using System.ComponentModel.DataAnnotations;

namespace Common.Entities
{
    /// <summary>
    /// Represents a user in the system
    /// </summary>
    public class User
    {
        /// <summary>
        /// The unique identifier for the user
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The username of the user
        /// </summary>
        /// <example>johndoe</example>
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; }

        /// <summary>
        /// The password of the user
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; }

        /// <summary>
        /// The first name of the user
        /// </summary>
        /// <example>John</example>
        [Required]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user
        /// </summary>
        /// <example>Doe</example>
        [Required]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; }

        /// <summary>
        /// Indicates whether the user has administrator privileges
        /// </summary>
        /// <example>false</example>
        [Required]
        public bool IsAdmin { get; set; }
    }
}
