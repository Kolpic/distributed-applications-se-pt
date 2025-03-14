using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    /// <summary>
    /// Represents a link between a project and a category (many-to-many relationship)
    /// </summary>
    public class ProjectCategory
    {
        /// <summary>
        /// The ID of the project
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// The ID of the category
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// The project associated with this relationship
        /// </summary>
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }

        /// <summary>
        /// The category associated with this relationship
        /// </summary>
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
    }
}
