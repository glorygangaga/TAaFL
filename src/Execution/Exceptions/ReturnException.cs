using Runtime;

namespace Execution.Exceptions;

/// <summary>
/// Внутреннее исключение библиотеки, используется для return из текущего цикла.
/// </summary>
#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.
internal class ReturnException : Exception
{
  public ReturnException(Value? value)
  {
    Value = value;
  }

  public Value? Value { get; }
}
#pragma warning restore RCS1194