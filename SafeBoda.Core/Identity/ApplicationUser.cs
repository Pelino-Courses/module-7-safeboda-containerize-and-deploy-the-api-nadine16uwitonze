using Microsoft.AspNetCore.Identity;

namespace SafeBoda.Core.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        
    }
}