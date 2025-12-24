namespace Semantics.Exceptions;

/// <summary>
/// Исключение из-за использования выражения, которое не допускается в текущем контексте.
/// </summary>
#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.
public class InvalidExpressionException : Exception
{
  public InvalidExpressionException(string message)
      : base(message)
  {
  }
}
#pragma warning restore RCS1194