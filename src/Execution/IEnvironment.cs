namespace Execution;

/// <summary>
/// Представляет окружение для выполнения программы.
/// Прежде всего это функции ввода/вывода.
/// </summary>
public interface IEnvironment
{
  public decimal ReadNumber();

  public void WriteNumber(decimal value);
}