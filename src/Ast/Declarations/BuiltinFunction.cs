using Runtime;

using ValueType = Runtime.ValueType;

namespace Ast.Declarations;

/// <summary>
/// Определение встроенной функции языка.
/// </summary>
public sealed class BuiltinFunction : AbstractFunctionDeclaration
{
  private readonly Func<List<Value>, Value> implementation;

  public BuiltinFunction(
      string name,
      IReadOnlyList<BuiltinFunctionParameter> parameters,
      ValueType resultType,
      Func<List<Value>, Value> implementation
  )
      : base(name, parameters)
  {
    ResultType = resultType;
    this.implementation = implementation;
  }

  public Value Invoke(List<Value> arguments)
  {
    return implementation(arguments);
  }

  public override void Accept(IAstVisitor visitor)
  {
    throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
  }
}