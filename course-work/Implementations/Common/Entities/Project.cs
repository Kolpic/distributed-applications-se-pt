using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    /// <summary>
    /// Represents a project in the system
    /// </summary>
    public class Project
    {
        /// <summary>
        /// The unique identifier for the project
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the user who owns this project
        /// </summary>
        [Required]
        public int OwnerId { get; set; }

        /// <summary>
        /// The title of the project
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Project title must be between 3 and 100 characters")]
        public string Title { get; set; }

        /// <summary>
        /// The description of the project
        /// </summary>
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; }

        /// <summary>
        /// The user who owns this project
        /// </summary>
        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }

        /// <summary>
        /// The categories associated with this project
        /// </summary>
        public ICollection<ProjectCategory> ProjectCategories { get; set; }

        /// <summary>
        /// The comments associated with this project
        /// </summary>
        public ICollection<Comment> Comments { get; set; }
    }
}
