using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeBoda.Core.Entities
{
    public class Rider
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = "TEXT")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "TEXT")]
        public string Email { get; set; } = string.Empty;

        
    }
}