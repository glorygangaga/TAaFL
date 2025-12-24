namespace Ast.Declarations;

public abstract class AbstractConstantDeclaration : Declaration
{
  protected AbstractConstantDeclaration(string name)
  {
    this.Name = name;
  }

  public string Name { get; }
}