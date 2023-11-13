namespace Rocky.BLL.DTOs;

// This is temportary and will be removed after adjusting viewModel template
public class ProductDto
{
    public AddProductDto Product { get; set; }
}

public class AddProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public string ShortDesc { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    public IFormFile File { get; set; }
    public int CategoryId { get; set; }
    public string? Image { get; set; } = string.Empty;
}