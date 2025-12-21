namespace Ast.Expressions;

public sealed class SwitchCase
{
  public SwitchCase(Expression value, Expression body)
  {
    Value = value;
    Body = body;
  }

  public Expression Value { get; }

  public Expression Body { get; }
}
