using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    /// <summary>
    /// Represents a project category
    /// </summary>
    public class Category
    {
        /// <summary>
        /// The unique identifier for the category
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the category
        /// </summary>
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 50 characters")]
        public string Name { get; set; }

        /// <summary>
        /// The description of the category
        /// </summary>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        /// <summary>
        /// Projects associated with this category
        /// </summary>
        public ICollection<ProjectCategory> ProjectCategories { get; set; }
    }
}
