using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.ViewModels.Tokens
{
    /// <summary>
    /// Data transfer object for user authentication
    /// </summary>
    public class PostVM
    {
        /// <summary>
        /// The username for authentication
        /// </summary>
        /// <example>johndoe</example>
        [Required(ErrorMessage = "Username is required!")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; }

        /// <summary>
        /// The password for authentication
        /// </summary>
        /// <example>SecureP@ssw0rd</example>
        [Required(ErrorMessage = "Password is required!")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; }
    }
}
