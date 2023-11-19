using Rocky.DAL.Models;

namespace Rocky.ViewModels;

public class ProductListViewModel
{
	public IEnumerable<Product> Products { get; set; } = new List<Product>();
	public int CurrentPage { get; set; } = 1;
	public int TotalPages { get; set; } = 0;
	public int ProductsCount { get; set; } = 0;
	public int ProductsPerPage { get; set;}
	public int NextPage { get; set; }
	public int PreviousPage { get; set;}
	public string OrderBy { get; set; } = "Id";
}
