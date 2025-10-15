namespace Lexer;

public class Token(
  TokenType type,
  TokenValue? value = null
)
{
  public TokenType Type { get; } = type;

  public TokenValue? Value { get; } = value;

  public override bool Equals(object? obj)
  {
    if (obj is not Token other)
    {
      return false;
    }

    if (Type != other.Type)
    {
      return false;
    }

    if (Value == null && other.Value == null)
    {
      return true;
    }

    return Value?.ToString() == other.Value?.ToString();
  }

  public override int GetHashCode()
  {
    return HashCode.Combine((int)Type, Value);
  }
}