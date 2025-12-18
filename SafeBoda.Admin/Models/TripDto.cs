using System;

namespace SafeBoda.Admin.Models
{
    public class TripDto
    {
        public Guid Id { get; set; }
        public Guid RiderId { get; set; }
        public Guid DriverId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}