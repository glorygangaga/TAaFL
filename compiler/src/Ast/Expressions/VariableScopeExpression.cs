using Ast.Declarations;

namespace Ast.Expressions;

public sealed class VariableScopeExpression : Expression
{
  private readonly List<VariableDeclaration> variables;

  public VariableScopeExpression(List<VariableDeclaration> variables, Expression expression)
  {
    this.variables = variables;
    Expression = expression;
  }

  public IReadOnlyList<VariableDeclaration> Variables => variables;

  public Expression Expression { get; }

  public override void Accept(IAstVisitor visitor)
  {
    visitor.Visit(this);
  }
}