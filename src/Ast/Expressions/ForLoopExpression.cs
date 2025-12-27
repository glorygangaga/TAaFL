using Ast.Declarations;

namespace Ast.Expressions;

public sealed class ForLoopExpression : Expression
{
  public ForLoopExpression(
        VariableDeclaration var,
        Expression endCondition,
        Expression? stepValue,
        Expression body
    )
  {
    VariableDeclaration = var;
    EndCondition = endCondition;
    StepValue = stepValue;
    Body = body;
  }

  public VariableDeclaration VariableDeclaration { get; }

  public Expression EndCondition { get; }

  public Expression? StepValue { get; }

  public Expression Body { get; }

  public override void Accept(IAstVisitor visitor)
  {
    visitor.Visit(this);
  }
}