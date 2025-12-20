namespace Ast.Expressions;

public enum BinaryOperation
{
  /// <summary>
  /// Операция сложения.
  /// </summary>
  Plus,

  /// <summary>
  /// Операция минус.
  /// </summary>
  Minus,

  /// <summary>
  /// Операция деления.
  /// </summary>
  Division,

  /// <summary>
  /// Операция умножения.
  /// </summary>
  Multiplication,

  /// <summary>
  /// Операция возведения в степень.
  /// </summary>
  Exponentiation,

  /// <summary>
  /// Операция остатка.
  /// </summary>
  Remainder,

  /// <summary>
  /// Операция целочисленный оператор деления.
  /// </summary>
  IntegerDivision,

  /// <summary>
  /// Операция логическое и.
  /// </summary>
  And,

  /// <summary>
  /// Операция логическое и.
  /// </summary>
  Or,

  /// <summary>
  /// Операция равно.
  /// </summary>
  Equal,

  /// <summary>
  /// Операция не равно.
  /// </summary>
  NotEqual,

  /// <summary>
  /// Операция меньше.
  /// </summary>
  LessThan,

  /// <summary>
  /// Операция меньше или равно.
  /// </summary>
  LessThanOrEqual,

  /// <summary>
  /// Операция больше.
  /// </summary>
  GreaterThan,

  /// <summary>
  /// Операция больше или равно.
  /// </summary>
  GreaterThanOrEqual,
}