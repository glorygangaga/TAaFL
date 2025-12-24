using Ast.Attributes;

namespace Ast.Declarations;

public sealed class ParameterDeclaration : AbstractParameterDeclaration
{
  private AstAttribute<AbstractTypeDeclaration?> declaredType;

  public ParameterDeclaration(string name, string typeName)
    : base(name)
  {
    this.TypeName = typeName;
  }

  public string TypeName { get; }

  public AbstractTypeDeclaration Type
  {
    get => declaredType.Get() ?? throw new InvalidOperationException(
        $"No declaration for parameter type {this.Type}"
    );
    set => declaredType.Set(value);
  }

  public override void Accept(IAstVisitor visitor)
  {
    visitor.Visit(this);
  }
}
