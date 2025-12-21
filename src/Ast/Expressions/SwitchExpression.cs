namespace Ast.Expressions;

public sealed class SwitchExpression : Expression
{
  public SwitchExpression(Expression expression, IReadOnlyList<SwitchCase> cases, Expression? defaultCase)
  {
    Expression = expression;
    Cases = cases;
    DefaultCase = defaultCase;
  }

  public Expression Expression { get; }

  public IReadOnlyList<SwitchCase> Cases { get; }

  public Expression? DefaultCase { get; }

  public override void Accept(IAstVisitor visitor)
  {
    visitor.Visit(this);
  }
}