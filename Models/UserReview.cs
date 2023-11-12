using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.Models;

public class UserReview
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	[Required, ForeignKey("User")]
	public int UserId { get; set; }

	[Required]
	public string Comment { get; set; } = string.Empty;

	[Required]
	public int Rating { get; set; } = 0;

	[Timestamp]
	public byte[] Timestamp { get; set; }

}