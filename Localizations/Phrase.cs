using Microsoft.Extensions.Localization;
using System.Reflection;

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
                return _localizer[key, arguments] ?? key;
            }
            catch
            {
                return key;
            }
        }
    }

}