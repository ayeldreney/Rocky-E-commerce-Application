using Microsoft.AspNetCore.Mvc.Rendering;

namespace Rocky.Models.ViewModels
{
    public class ProductViewModel
    {


        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategorySelectList { get; set; }


    }
}
