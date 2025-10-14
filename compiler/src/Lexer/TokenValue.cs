namespace Lexer;

public class TokenValue(string tokenValue)
{
  private readonly object value = tokenValue;

  public override string ToString()
  {
    return value switch
    {
      string s => s,
      decimal d => d.ToString(),
      _ => throw new NotImplementedException()
    };
  }

  public decimal ToDemical()
  {
    return value switch
    {
      string s => decimal.Parse(s),
      decimal d => d,
      _ => throw new NotImplementedException()
    };
  }
}