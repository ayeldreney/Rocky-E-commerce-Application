using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.DAL.Models;

public class Cart
{
	[Key]
	public int Id { get; set; }

	[Required, ForeignKey("User")]
	public string UserId { get; set; }

	[Timestamp]
	public int Timestamp { get; set; }

	public virtual HashSet<CartItem> Items { get; set; } = new HashSet<CartItem>();

	public virtual AppUser User { get; set; }
}