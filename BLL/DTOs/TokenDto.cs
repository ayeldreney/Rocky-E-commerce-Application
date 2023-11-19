namespace Rocky.BLL.DTOs;

public class TokenDto
{
	public string Token { get; set; }
	public DateTime ValidTo { get; internal set; }
}
