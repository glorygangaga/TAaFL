using Execution;

namespace Parser;

/// <summary>
/// Поддельное окружение: работает как настоящее, но не совершает реального ввода/вывода.
/// </summary>
public class FakeEnvironment : IEnvironment
{
  private readonly List<decimal> results = [];

  public IReadOnlyList<decimal> Results => results;

  public void WriteNumber(decimal result)
  {
    results.Add(result);
  }

  public decimal ReadNumber()
  {
    decimal number = results.Last();
    results.RemoveAt(results.Count - 1);
    return number;
  }
}