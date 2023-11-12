using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volga.Infrastructure.Enums;

namespace Rocky.Models;

public class Cart
{
	[Key]
	public int Id { get; set; }

	[Required, ForeignKey("User")]
	public int UserId { get; set; }

	[EnumDataType(typeof(CartStatus))]
	public CartStatus Status { get; set; }

	[Timestamp]
	public int Timestamp { get; set; }

	public virtual IEnumerable<CartItem> Items { get; set; } = Enumerable.Empty<CartItem>();

	public virtual User User { get; set; }
}