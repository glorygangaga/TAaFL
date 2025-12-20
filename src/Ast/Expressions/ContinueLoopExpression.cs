namespace Ast.Expressions;

public sealed class ContinueLoopExpression : Expression
{
  public override void Accept(IAstVisitor visitor)
  {
    visitor.Visit(this);
  }
}