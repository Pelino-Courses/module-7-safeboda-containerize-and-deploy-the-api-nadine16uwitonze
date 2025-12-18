using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeBoda.Core.Entities
{
    public class Trip
    {
        [Key]
        public Guid Id { get; set; }

       
        [Required]
        public Guid RiderId { get; set; }

        [ForeignKey("RiderId")]
        public Rider? Rider { get; set; }  

      
        [Required]
        public Guid DriverId { get; set; }

        [ForeignKey("DriverId")]
        public Driver? Driver { get; set; }  

        [Required]
        public decimal Amount { get; set; }  

        [Required]
        public DateTime Date { get; set; }  

        [Required]
        public bool IsActive { get; set; }  
    }
}