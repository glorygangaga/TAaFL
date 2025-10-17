namespace Parser;

public class Parser
{
  private readonly TokenStream tokens;

  private Parser(string code)
  {
    tokens = new TokenStream(code);
  }

  public int EvaluateExpression(string code)
  {
    Parser p = new(code);
    return p.ParseExpression();
  }

  private int ParseExpression()
  {
    return 1;
  }
}
