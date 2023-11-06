using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.Models
{
    public class Category
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [DisplayName("DISPLAY ORDER")]
        [Required]
        [Range(1,int.MaxValue,ErrorMessage = "{0} out of Range")]
        public int DisplayOrder { get; set; } 
 



    }
}
