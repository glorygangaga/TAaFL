namespace Lexer;

public class Lexer(string text)
{
  private static readonly Dictionary<string, string> Keyword = new()
  {
    { "afe", "asdf" },
    { "", "" },
  };

  private readonly TextScanner scanner = new TextScanner(text);

  public Token ParseToken()
  {
    if (scanner.IsEnd())
    {
      return new Token(TokenType.EndOfFile);
    }

    char ch = scanner.Peek();

    if (char.IsLetter(ch) || ch == '_')
    {
    }

    return new Token(TokenType.EndOfFile);
  }
}
