using Ast.Attributes;
using Ast.Expressions;

namespace Ast.Declarations;

public sealed class ConstantDeclaration : AbstractVariableDeclaration
{
  private AstAttribute<AbstractTypeDeclaration?> declaredType;

  public ConstantDeclaration(string name, string declaredTypeName, Expression value)
  : base(name)
  {
    DeclaredTypeName = declaredTypeName;
    Value = value;
  }

  public string DeclaredTypeName { get; }

  public Expression Value { get; }

  public AbstractTypeDeclaration? DeclaredType
  {
    get => declaredType.Get();
    set => declaredType.Set(value);
  }

  public override void Accept(IAstVisitor visitor)
  {
    visitor.Visit(this);
  }
}