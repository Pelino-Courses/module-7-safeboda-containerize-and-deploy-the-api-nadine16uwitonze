using System;

namespace SafeBoda.Application.DTOs
{
    public class DriverDto
    {
        public Guid Id { get; set; }                     
        public string Name { get; set; } = string.Empty; 
        public string PhoneNumber { get; set; } = string.Empty; 
        public string VehicleNumber { get; set; } = string.Empty; 
        public string VehicleType { get; set; } = string.Empty;   
        public string LicenseNumber { get; set; } = string.Empty; 
    }
}