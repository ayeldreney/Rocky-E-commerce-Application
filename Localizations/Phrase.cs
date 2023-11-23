using Microsoft.Extensions.Localization;
using System.Reflection;
using System.Web;

namespace Rocky.Localizations;
public class Phrase
{
	private readonly IStringLocalizerFactory _factory;
	private IStringLocalizer? _localizer = null;

	public Phrase(IStringLocalizerFactory factory)
	{
		_factory = factory;
	}

	public string this[string baseResourceName, string key, params object[] arguments]
	{
		get
		{
			var assemblyName = new AssemblyName(typeof(Phrase).GetTypeInfo().Assembly.FullName!);
			try
			{
				_localizer = _factory.Create(baseResourceName, assemblyName.Name!);
				string ph = _localizer[key, arguments] ?? key;
				//ph = HttpUtility.HtmlDecode(ph);
				return ph;
			}
			catch
			{
				return key;
			}
		}
	}

}