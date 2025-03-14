using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.ViewModels.Comments
{
    /// <summary>
    /// Data transfer object for displaying project comments
    /// </summary>
    public class ProjectCommenentsDTO
    {
        /// <summary>
        /// The content of the comment
        /// </summary>
        /// <example>This is a great project idea!</example>
        [StringLength(1000)]
        public string Content { get; set; }

        /// <summary>
        /// The username of the comment author
        /// </summary>
        /// <example>johndoe</example>
        [StringLength(50)]
        public string Username { get; set; }
    }
}
