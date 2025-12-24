namespace Semantics.Exceptions;

/// <summary>
/// Исключение из-за некорректного использования символа (функции, переменной, типа).
/// </summary>
#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.
public class InvalidSymbolException : Exception
{
  public InvalidSymbolException(string name, string expectedCategory, string actualCategory)
      : base($"Name {name} should refer to a {expectedCategory}, got {actualCategory}")
  {
  }
}
#pragma warning restore RCS1194