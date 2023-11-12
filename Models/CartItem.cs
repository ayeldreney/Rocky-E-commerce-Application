using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.Models;

public class CartItem
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	[ForeignKey("Cart")]
	public int CartId { get; set; }

	[ForeignKey("Product")]
	public int ProductId { get; set; }

	public uint Qty { get; set; } = 0;

	public virtual Product Product { get; set; }

	public virtual Cart Cart { get; set; }
}