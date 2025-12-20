namespace Ast.Expressions;

public sealed class PrintExpression(Expression value) : Expression
{
  public Expression Value { get; } = value;

  public override void Accept(IAstVisitor visitor)
  {
    visitor.Visit(this);
  }
}