namespace Ast.Expressions;

public sealed class SequenceExpression : Expression
{
  private readonly List<Expression> sequence;

  public SequenceExpression(List<Expression> sequence)
  {
    this.sequence = sequence;
  }

  public IReadOnlyList<Expression> Sequence => sequence;

  public override void Accept(IAstVisitor visitor)
  {
    visitor.Visit(this);
  }
}