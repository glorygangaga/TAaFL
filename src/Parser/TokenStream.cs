using Lexer;

using Lex = Lexer.Lexer;

namespace Parser;

public class TokenStream
{
  private readonly Lex lexer;
  private readonly Queue<Token> buffer = new();

  public TokenStream(string text, int lookahead = 2)
  {
    lexer = new Lex(text);
    for (int i = 0; i < lookahead; i++)
    {
      buffer.Enqueue(lexer.ParseToken());
    }
  }

  public Token Peek(int k = 0)
  {
    if (k < 0 || k >= buffer.Count)
    {
      throw new ArgumentOutOfRangeException(nameof(k));
    }

    return buffer.ElementAt(k);
  }

  public void Advance()
  {
    buffer.Dequeue();
    buffer.Enqueue(lexer.ParseToken());
  }
}