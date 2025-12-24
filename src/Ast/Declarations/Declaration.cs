using Ast.Attributes;

using ValueType = Runtime.ValueType;

namespace Ast.Declarations;

/// <summary>
/// Абстрактный класс всех объявлений (declarations).
/// </summary>
public abstract class Declaration : AstNode
{
  private AstAttribute<ValueType> resultType;

  /// <summary>
  /// Тип результата объявления.
  /// </summary>
  public ValueType ResultType
  {
    get => resultType.Get();

    set => resultType.Set(value);
  }
}