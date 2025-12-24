using Ast.Declarations;

using Runtime;

namespace Execution;

/// <summary>
/// Контекст выполнения программы (все переменные, константы и другие символы).
/// </summary>
public class Context
{
  private readonly IEnvironment environment;
  private readonly Stack<Scope> scopes = [];
  private readonly Dictionary<string, Value> constants = [];
  private readonly Dictionary<string, FunctionDeclaration> functions = [];

  public Context(IEnvironment environment)
  {
    this.environment = environment;
    scopes.Push(new Scope());
  }

  public IEnvironment Environment => environment;

  public void PushScope(Scope scope)
  {
    scopes.Push(scope);
  }

  public void PopScope()
  {
    scopes.Pop();
  }

  /// <summary>
  /// Возвращает значение переменной или константы.
  /// </summary>
  public Value GetValue(string name)
  {
    foreach (Scope s in scopes)
    {
      if (s.TryGetVariable(name, out Value? variable) && variable is not null)
      {
        return variable;
      }
    }

    if (constants.TryGetValue(name, out Value? constant))
    {
      return constant;
    }

    throw new ArgumentException($"Variable '{name}' is not defined");
  }

  /// <summary>
  /// Присваивает (изменяет) значение переменной.
  /// </summary>
  public void AssignVariable(string name, Value value)
  {
    foreach (Scope s in scopes.Reverse())
    {
      if (s.TryAssignVariable(name, value))
      {
        return;
      }
    }

    throw new ArgumentException($"Variable '{name}' is not defined");
  }

  /// <summary>
  /// Определяет переменную в текущей области видимости.
  /// </summary>
  public void DefineVariable(string name, Value value)
  {
    if (!scopes.Peek().TryDefineVariable(name, value))
    {
      throw new ArgumentException($"Variable '{name}' is already defined in this scope");
    }
  }

  /// <summary>
  /// Определяет константу в глобальной области видимости.
  /// </summary>
  public void DefineConstant(string name, Value value)
  {
    if (!constants.TryAdd(name, value))
    {
      throw new ArgumentException($"Constant '{name}' is already defined");
    }
  }

  public FunctionDeclaration GetFunction(string name)
  {
    if (functions.TryGetValue(name, out FunctionDeclaration? function))
    {
      return function;
    }

    throw new ArgumentException($"Function '{name}' is not defined");
  }

  public void DefineFunction(FunctionDeclaration function)
  {
    if (!functions.TryAdd(function.Name, function))
    {
      throw new ArgumentException($"Function '{function.Name}' is already defined");
    }
  }
}