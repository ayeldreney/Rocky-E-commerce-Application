namespace Rocky.BLL.Languages;

public class EnglishUS : LanguageBase
{
	public override string Name { get; } = "English";
	public override string Code { get; } = "en-US";
	public override string LangNameInNative { get; } = "English US";
	public override string FlagImageName { get; } = @"us.png";
}