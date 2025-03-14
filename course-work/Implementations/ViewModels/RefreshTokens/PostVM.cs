using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.ViewModels.RefreshTokens
{
    /// <summary>
    /// Data transfer object for refreshing an authentication token
    /// </summary>
    public class PostVM
    {
        /// <summary>
        /// The refresh token used to obtain a new JWT token
        /// </summary>
        /// <example>9f8d7c6b-5a4e-3b2d-1c0f-9e8d7c6b5a4e</example>
        [Required(ErrorMessage = "Refresh token is required!")]
        [StringLength(255, ErrorMessage = "Token cannot exceed 255 characters")]
        public string RefreshToken { get; set; }
    }
}
