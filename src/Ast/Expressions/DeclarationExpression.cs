using Ast.Declarations;

namespace Ast.Expressions;

public sealed class DeclarationExpression : Expression
{
  public DeclarationExpression(Declaration declaration)
  {
    Declaration = declaration;
  }

  public Declaration Declaration { get; }

  public override void Accept(IAstVisitor visitor)
  {
    visitor.Visit(this);
  }
}
