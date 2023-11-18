using Rocky.DAL.Models;
using Rocky.ViewModels;
namespace Rocky.ViewModels
{
    public class DetailsViewModel
    {

        public DetailsViewModel()
        {
            ProductViewModel productViewModel = new ProductViewModel(); 
          
           productViewModel.Product= new Product()
           {
               Id = productViewModel.Product.Id,
               Name = productViewModel.Product.Name,
               ShortDesc = productViewModel.Product.ShortDesc,
               Description = productViewModel.Product.Description,
               Price = productViewModel.Product.Price,
               Image = productViewModel.Product.Image,
               CategoryId = productViewModel.Product.CategoryId,
           };


            Product = productViewModel.Product;

        

        }


        public Product Product { get; set; }    

        public bool ExistsInCart { get; set; }  


    }
}
