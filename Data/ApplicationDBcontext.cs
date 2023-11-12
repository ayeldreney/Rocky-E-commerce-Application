using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rocky.Models;

namespace Rocky.Data;

public class ApplicationDBcontext : IdentityDbContext<User>
{
	public ApplicationDBcontext(DbContextOptions<ApplicationDBcontext> options):base(options) {}

	public DbSet<Category> Categories { get; set; }
	public DbSet<ApplicationType> ApplicationType { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderLine> OrderLines { get; set; }
	public DbSet<UserReview> UserReviews { get; set; }
	public DbSet<Cart> Carts { get; set; }
	public DbSet<CartItem> CartItems { get; set; }
}