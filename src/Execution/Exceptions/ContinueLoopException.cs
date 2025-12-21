namespace Execution.Exceptions;

/// <summary>
/// Внутреннее исключение библиотеки, используется для continue из текущего цикла.
/// </summary>
#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.
public sealed class ContinueLoopException : Exception
{
  public ContinueLoopException()
      : base("Loop continue")
  {
  }
}
#pragma warning restore RCS1194