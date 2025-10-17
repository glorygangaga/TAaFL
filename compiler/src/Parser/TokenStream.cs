using Lexer;

using Lex = Lexer.Lexer;

namespace Parser;

public class TokenStream
{
  private readonly Lex lexer;
  private Token token;

  public TokenStream(string text)
  {
    lexer = new Lex(text);
    token = lexer.ParseToken();
  }

  public Token Peek()
  {
    return token;
  }

  public void Advance()
  {
    token = lexer.ParseToken();
  }
}