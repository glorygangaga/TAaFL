using Runtime;

namespace Execution;

/// <summary>
/// Представляет окружение для выполнения программы.
/// Прежде всего это функции ввода/вывода.
/// </summary>
public interface IEnvironment
{
  public Value Read();

  public void Write(Value value);
}