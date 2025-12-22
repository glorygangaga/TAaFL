namespace Ast.Expressions;

public sealed class PrintExpression : Expression
{
  public PrintExpression(IReadOnlyList<Expression> values)
  {
    Values = values;
  }

  public IReadOnlyList<Expression> Values { get; }

  public override void Accept(IAstVisitor visitor)
  {
    visitor.Visit(this);
  }
}