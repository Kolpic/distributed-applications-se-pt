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
    /// Represents a refresh token for JWT authentication
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// Represents the status of a refresh token
        /// </summary>
        public enum TokenStatus
        {
            /// <summary>
            /// Token is pending and can be used
            /// </summary>
            Pending = 0,

            /// <summary>
            /// Token has been used and is no longer valid
            /// </summary>
            Used = 1
        }

        /// <summary>
        /// The unique identifier for the refresh token
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the user this token is associated with
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// The token value
        /// </summary>
        [Required]
        [StringLength(255, ErrorMessage = "Token cannot exceed 255 characters")]
        public string Token { get; set; }

        /// <summary>
        /// The current status of the token
        /// </summary>
        [Required]
        public TokenStatus Status { get; set; }

        /// <summary>
        /// The user this token is associated with
        /// </summary>

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
      
    }
}
