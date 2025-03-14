using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.ViewModels.Users
{
    /// <summary>
    /// Data transfer object for updating an existing user
    /// </summary>
    public class EditUserDTO
    {
        /// <summary>
        /// The unique identifier of the user to update
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "User ID is required!")]
        public int Id { get; set; }

        /// <summary>
        /// The updated username
        /// </summary>
        /// <example>johndoe</example>
        [Required(ErrorMessage = "Username is required!")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; }

        /// <summary>
        /// The updated password
        /// </summary>
        /// <example>NewSecureP@ssw0rd</example>
        [Required(ErrorMessage = "Password is required!")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; }

        /// <summary>
        /// The updated first name
        /// </summary>
        /// <example>John</example>
        [Required(ErrorMessage = "First name is required!")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; }

        /// <summary>
        /// The updated last name
        /// </summary>
        /// <example>Doe</example>
        [Required(ErrorMessage = "Last name is required!")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; }
    }
}
