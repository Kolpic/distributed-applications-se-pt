using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.ViewModels.Projects
{
    /// <summary>
    /// Data transfer object for creating a new project
    /// </summary>
    public class CreateProjectDTO
    {
        /// <summary>
        /// The ID of the user who owns the project (automatically set to current user)
        /// </summary>
        /// <example>1</example>
        public int OwnerId { get; set; }

        /// <summary>
        /// The title of the project
        /// </summary>
        /// <example>E-commerce Platform</example>
        [Required(ErrorMessage = "Project title is required!")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Project title must be between 3 and 100 characters")]
        public string Title { get; set; }

        /// <summary>
        /// The description of the project
        /// </summary>
        /// <example>A comprehensive e-commerce platform with product management, shopping cart, and payment processing</example>
        [Required(ErrorMessage = "Project description is required!")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; }
    }
}
