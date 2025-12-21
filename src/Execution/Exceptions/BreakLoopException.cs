namespace Execution.Exceptions;

/// <summary>
/// Внутреннее исключение библиотеки, используется для выхода из текущего цикла.
/// </summary>
#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.
internal class BreakLoopException : Exception
{
  public BreakLoopException()
      : base("Loop break")
  {
  }
}
#pragma warning restore RCS1194