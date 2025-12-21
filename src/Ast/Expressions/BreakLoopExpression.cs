namespace Ast.Expressions;

public sealed class BreakLoopExpression : Expression
{
  public override void Accept(IAstVisitor visitor)
  {
    visitor.Visit(this);
  }
}