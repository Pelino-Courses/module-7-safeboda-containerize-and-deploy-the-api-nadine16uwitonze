using System;

namespace SafeBoda.Application.DTOs
{
    public class RiderDto
    {
        public Guid Id { get; set; }              
        public string Name { get; set; } = string.Empty;   
        public string Email { get; set; } = string.Empty;  
       
    }
}