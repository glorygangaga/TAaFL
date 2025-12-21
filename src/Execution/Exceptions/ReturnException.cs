namespace Execution.Exceptions;

/// <summary>
/// Внутреннее исключение библиотеки, используется для return из текущего цикла.
/// </summary>
#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.
internal class ReturnException : Exception
{
  public ReturnException(decimal? value)
  {
    Value = value;
  }

  public decimal? Value { get; }
}
#pragma warning restore RCS1194