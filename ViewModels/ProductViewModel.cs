using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky.DAL.Models;

namespace Rocky.ViewModels
{
	public class ProductViewModel
	{


		public Product Product { get; set; }
		public IEnumerable<SelectListItem> CategorySelectList { get; set; }


	}
}
