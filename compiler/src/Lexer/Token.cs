namespace Lexer;

public class Token(
  TokenType type,
  TokenValue? value = null
)
{
  public TokenType Type { get; } = type;

  public TokenValue? Value { get; } = value;
}