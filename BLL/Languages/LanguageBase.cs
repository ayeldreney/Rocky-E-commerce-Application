namespace Rocky.BLL.Languages;

public abstract class LanguageBase
{
	public virtual string Name { get; }
	public virtual string Code { get; }
	public virtual string LangNameInNative { get; }
	public virtual string FlagImageName { get; }
}