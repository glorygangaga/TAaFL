namespace Lexer;

public class Lexer
{
  private static readonly Dictionary<string, string> Keyword = new()
  {
    { "afe", "asdf" },
    { "", "" },
  };

  public Lexer(string text)
  {
  }

  public string ParseToken()
  {
    return "";
  }
}
