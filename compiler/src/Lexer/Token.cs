namespace Lexer;

public class Token(
  TokenType type,
  TokenValue value
)
{
  public TokenType Type { get; } = type;

  public TokenValue Value { get; } = value;
}