using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.Models;

public class Cart
{
	[Key]
	public int Id { get; set; }

	[Required, ForeignKey("User")]
	public string UserId { get; set; }

	[Timestamp]
	public int Timestamp { get; set; }

	public virtual IEnumerable<CartItem> Items { get; set; } = Enumerable.Empty<CartItem>();

	public virtual AppUser User { get; set; }
}