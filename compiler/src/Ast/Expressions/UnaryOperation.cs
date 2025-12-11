namespace Ast.Expressions;

public enum UnaryOperation
{
  /// <summary>
  /// Унарный плюс
  /// </summary>
  Plus,

  /// <summary>
  /// Унарный минус
  /// </summary>
  Minus,

  /// <summary>
  /// Унарный нет
  /// </summary>
  Not,

  /// <summary>
  /// Арифметический оператор инкремента "++"
  /// </summary>
  Increment,

  /// <summary>
  /// Арифметический оператор декремента "--"
  /// </summary>
  Decrement,
}