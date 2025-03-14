using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.ViewModels.Users
{
    /// <summary>
    /// Data transfer object for creating a new user
    /// </summary>
    public class CreateUserDTO
    {
        /// <summary>
        /// The username for the new user
        /// </summary>
        /// <example>johndoe</example>
        [Required(ErrorMessage = "This field is Required!")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; }

        /// <summary>
        /// The password for the new user
        /// </summary>
        /// <example>SecureP@ssw0rd</example>
        [Required(ErrorMessage = "This field is Required!")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; }

        /// <summary>
        /// The first name of the new user
        /// </summary>
        /// <example>John</example>
        [Required(ErrorMessage = "This field is Required!")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the new user
        /// </summary>
        /// <example>Doe</example>
        [Required(ErrorMessage = "This field is Required!")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; }

        /// <summary>
        /// Indicates whether the new user has administrator privileges
        /// </summary>
        /// <example>false</example>
        [Required(ErrorMessage = "IsAdmin status is required!")]
        public bool IsAdmin { get; set; }
    }
}
