using Ast.Attributes;

using Ast.Expressions;

namespace Ast.Declarations;

public sealed class FunctionDeclaration : AbstractFunctionDeclaration
{
  private AstAttribute<AbstractTypeDeclaration?> declaredType;

  public FunctionDeclaration(
        string name,
        IReadOnlyList<ParameterDeclaration> parameters,
        string? declaredTypeName,
        Expression body
    )
        : base(name, parameters)
  {
    DeclaredTypeName = declaredTypeName;
    Body = body;
  }

  public string? DeclaredTypeName { get; }

  public AbstractTypeDeclaration? DeclaredType
  {
    get => declaredType.Get();
    set => declaredType.Set(value);
  }

  public Expression Body { get; }

  public override void Accept(IAstVisitor visitor)
  {
    visitor.Visit(this);
  }
}