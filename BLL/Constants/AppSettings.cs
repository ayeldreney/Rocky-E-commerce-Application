
using Rocky.BLL.Languages;

namespace Rocky.BLL.Constants;

public class AppSettings
{
	public static string LocalizationResourcesPath = "Localizations.Phrases";
	public static string LocalizationCulture = "en-US";
	public static bool isRightToLeft = false;
	public static int DateUTCAdjustment = +2;

	public static class UserSettings
	{
		public static int MaxFailedAccessAttempts = 5;
		public static int LockoutTimeSpanInMinutes = 5;
		public static string TokenBearerCookieName = "RockyAuth";
	}

	public static class Culture
	{
		public static string[] supportedCultures = new string[] {
			"en-US",
			"ar-EG"
		};

		public static Dictionary<string, LanguageBase> SupportedCulturesDetailed = new Dictionary<string, LanguageBase>
		{
			{ "ar-EG",  new ArabicEG() },
			{ "en-US",  new EnglishUS() },
		};
	}

	public static class Product
	{
		public static string ProductImagePath = @"/images/product/";
		public static int ProductsPerPage = 8;
		public static SortListBase[] SortByList = new SortListBase[]
		{
			new SortListBase() { Phrase = "DateOldNew", Column = "Id", Order = "ASC" },
			new SortListBase() { Phrase = "DateNewOld", Column = "Id", Order = "DESC" },
			new SortListBase() { Phrase = "AlphaAZ", Column = "Name", Order = "ASC" },
			new SortListBase() { Phrase = "AlphaZA", Column = "Name", Order = "DESC" },
			new SortListBase() { Phrase = "PriceLowHigh", Column = "Price", Order = "ASC" },
			new SortListBase() { Phrase = "PriceHighLow", Column = "Price", Order = "DESC" },
		};

	}
}