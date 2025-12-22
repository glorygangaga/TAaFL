namespace Ast.Expressions;

public sealed class EmptyExpression : Expression
{
  public override void Accept(IAstVisitor visitor)
  {
    visitor.Visit(this);
  }
}