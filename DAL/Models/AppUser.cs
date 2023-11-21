using Microsoft.AspNetCore.Identity;

namespace Rocky.DAL.Models;
public class AppUser : IdentityUser
{
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string? Address { get; set; }

	public virtual HashSet<Wishlist> Wishlists { get; set; } = new HashSet<Wishlist>();

	public virtual HashSet<Order> Orders { get; set; } = new HashSet<Order>();

	public virtual Cart Cart { get; set; }
}