using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.DAL.Models;

public class Wishlist
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	[Required]
	public string UserId { get; set; } = string.Empty;

	[ForeignKey("UserId")]
	public virtual AppUser User { get; set; }

	public virtual HashSet<Product> Products { get; set; } = new HashSet<Product>();
}
