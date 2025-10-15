namespace Lexer;

public class TokenValue
{
  private readonly object value;

  public TokenValue(string value) => this.value = value;

  public TokenValue(decimal value) => this.value = value;

  public override string ToString() =>
      value switch
      {
        string s => s,
        decimal d => d.ToString(),
        _ => throw new NotImplementedException()
      };

  public decimal ToDecimal() =>
      value switch
      {
        string s => decimal.Parse(s),
        decimal d => d,
        _ => throw new NotImplementedException()
      };

  public override bool Equals(object? obj) =>
      obj is TokenValue other && Equals(value, other.value);

  public override int GetHashCode() => value.GetHashCode();
}
