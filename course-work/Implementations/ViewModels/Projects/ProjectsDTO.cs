using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.ViewModels.Projects
{
    /// <summary>
    /// Data transfer object for displaying project details with related data
    /// </summary>
    public class ProjectsDTO
    {
        /// <summary>
        /// The unique identifier of the project
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// The name/title of the project
        /// </summary>
        /// <example>E-commerce Platform</example>
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// The username of the project owner
        /// </summary>
        /// <example>johndoe</example>
        [StringLength(50)]
        public string OwnerName { get; set; }

        /// <summary>
        /// The list of category names associated with this project
        /// </summary>
        /// <example>["Frontend", "API", "Database"]</example>
        public List<string> Categories { get; set; }

        /// <summary>
        /// The list of comments associated with this project
        /// </summary>
        /// <example>["Great project!", "Need to add more features"]</example>
        public List<string> Comments { get; set; }
    }
}
