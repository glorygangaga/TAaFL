namespace Semantics.Exceptions;

/// <summary>
/// Исключение из-за некорректного обращения к переменной.
/// </summary>
#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.
public class InvalidAssignmentException : Exception
{
  public InvalidAssignmentException(string message)
      : base(message)
  {
  }
}
#pragma warning restore RCS1194