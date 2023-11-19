namespace Rocky.BLL.Helpers;

using Rocky.DAL.Data;
using Rocky.DAL.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class LoremDataGenerator
{
	private readonly ApplicationDBcontext _dbContext; // Replace with your DbContext type

	public LoremDataGenerator(ApplicationDBcontext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task GenerateAndAddDataAsync(int numberOfItems)
	{
		var loremIpsum = new Bogus.DataSets.Lorem();

		for (int i = 0; i < numberOfItems; i++)
		{
			var model = new Product(); // Replace with your model class
			model.Name = loremIpsum.Sentence();
			model.ShortDesc = loremIpsum.Paragraph();
			model.Image = await DownloadAndSaveImageAsync();
			model.CategoryId = GetRandomCategoryId();
			model.Price = new Random().NextInt64();

			_dbContext.Products.Add(model);
		}

		await _dbContext.SaveChangesAsync();
	}

	public int GetRandomCategoryId()
	{
		// Ensure the Category table is not empty
		if (_dbContext.Categories.Any())
		{
			var random = new Random();
			var randomCategory = _dbContext.Categories
				.OrderBy(c => Guid.NewGuid())
				.FirstOrDefault();

			if (randomCategory != null)
			{
				return randomCategory.Id;
			}
		}

		// Return a default value or handle as needed
		return -1; // You can choose a default value or throw an exception
	}

	private async Task<string> DownloadAndSaveImageAsync()
	{
		var _imageDirectory = @"/Project\Rocky-E-commerce-Application\wwwroot\Images\product\";
		using (var httpClient = new HttpClient())
		{
			try
			{
				var guid = Guid.NewGuid();
				var fileName = $"{guid}.jpg";
				var filePath = Path.Combine(_imageDirectory, fileName);

				var response = await httpClient.GetAsync("https://picsum.photos/200/300");
				response.EnsureSuccessStatusCode();

				using (var fileStream = File.Create(filePath))
				{
					await response.Content.CopyToAsync(fileStream);
					return fileName;
				}
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine($"Error fetching image: {ex.Message}");
				return null;
			}
		}
	}
}