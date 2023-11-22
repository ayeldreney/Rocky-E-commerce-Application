using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.DAL.Models;

public class Product
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	[Required, MaxLength(500)]
	public string Name { get; set; } = string.Empty;

	public string ShortDesc { get; set; } = string.Empty;

	[Required, MaxLength(1000)]
	public string Description { get; set; } = string.Empty;

	[MaxLength(20)]
	public double Price { get; set; }

	public string Image { get; set; } = string.Empty;

	public string? Vendor { get; set; }

	public string? Unit { get; set; }

	[Display(Name = "Category Type")]
	public int CategoryId { get; set; }

	[ForeignKey("CategoryId")]
	public virtual Category Category { get; set; }

	public virtual HashSet<Wishlist> Wishlists { get; set; } = new HashSet<Wishlist>();

	public virtual ICollection<UserReview> UserReviews { get; set; } = new HashSet<UserReview>();
}