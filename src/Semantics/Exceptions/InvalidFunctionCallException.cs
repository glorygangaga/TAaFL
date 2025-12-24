namespace Semantics.Exceptions;

/// <summary>
/// Исключение из-за некорректного вызова функции.
/// </summary>
#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.
public class InvalidFunctionCallException : Exception
{
  public InvalidFunctionCallException(string message)
      : base(message)
  {
  }
}
#pragma warning restore RCS1194