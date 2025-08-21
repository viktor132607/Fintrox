using Microsoft.AspNetCore.Identity;

namespace Fintrox.Data;
public class ApplicationUser : IdentityUser
{
	public string? DisplayName { get; set; }
}
