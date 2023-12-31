using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.DAL.Models;

public class UserReview
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	[ForeignKey("User")]
	public string? UserId { get; set; }

	[Required]
	public string Comment { get; set; } = string.Empty;

	[Required, MaxLength(2)]
	public int Rating { get; set; } = 0;

	[Required]
	public int ProductId { get; set; }

	[Timestamp]
	public byte[] Timestamp { get; set; }

	[ForeignKey("ProductId")]
	public virtual Product Product { get; set; }

	[ForeignKey("UserId")]
	public virtual AppUser User { get; set; }

}