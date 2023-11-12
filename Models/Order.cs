using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.Models;

public class Order
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	[Required, ForeignKey("User")]
	public string UserId { get; set; }

	[Timestamp]
	public byte[] Timestamp { get; set; }

	public virtual User User { get; set; }
	public virtual IEnumerable<OrderLine> OrderLines { get; set; } = Enumerable.Empty<OrderLine>();
}