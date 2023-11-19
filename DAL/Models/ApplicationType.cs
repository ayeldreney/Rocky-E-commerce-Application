using System.ComponentModel.DataAnnotations;

namespace Rocky.DAL.Models
{
	public class ApplicationType
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }

	}
}
