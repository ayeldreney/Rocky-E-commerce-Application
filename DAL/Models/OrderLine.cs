using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.DAL.Models;

public class OrderLine
{
	public int Id { get; set; }

	[Required, ForeignKey("Order")]
	public int OrderId { get; set; }

	[Required, ForeignKey("Product")]
	public int ProductId { get; set; }

	[Required]
	public double Price { get; set; } = 0;

	[Required]
	public int Qty { get; set; } = 0;

	public virtual Order Order { get; set; }

	public virtual Product Product { get; set; }
}