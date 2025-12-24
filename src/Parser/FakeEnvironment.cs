using Execution;

using Runtime;

namespace Parser;

/// <summary>
/// Поддельное окружение: работает как настоящее, но не совершает реального ввода/вывода.
/// </summary>
public class FakeEnvironment : IEnvironment
{
  private readonly List<Value> results = [];

  public IReadOnlyList<Value> Results => results;

  public void Write(Value result)
  {
    results.Add(result);
  }

  public Value Read()
  {
    Value number = results.Last();
    results.RemoveAt(results.Count - 1);
    return number;
  }
}