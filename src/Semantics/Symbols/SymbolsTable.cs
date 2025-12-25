using System.Diagnostics;

using Ast.Declarations;

using Semantics.Exceptions;

using ValueType = Runtime.ValueType;

namespace Semantics.Symbols;

/// <summary>
/// Таблица символов, основанная на лексических областях видимости (областях действия) символов в коде.
/// </summary>
public sealed class SymbolsTable
{
  private readonly SymbolsTable? parent;

  private readonly Dictionary<string, AbstractVariableDeclaration> variables;
  private readonly Dictionary<string, List<AbstractFunctionDeclaration>> functions;
  private readonly Dictionary<string, AbstractTypeDeclaration> types;

  public SymbolsTable(SymbolsTable? parent)
  {
    this.parent = parent;
    variables = [];
    functions = [];
    types = [];
  }

  public SymbolsTable? Parent => parent;

  public AbstractVariableDeclaration GetVariableDeclaration(string name)
  {
    if (variables.TryGetValue(name, out AbstractVariableDeclaration? variable))
    {
      return variable;
    }

    return parent?.GetVariableDeclaration(name)
      ?? throw UnknownSymbolException.UndefinedVariableOrFunction(name);
  }

  public AbstractFunctionDeclaration GetFunctionDeclaration(
  string name,
  IReadOnlyList<ValueType> argumentTypes)
  {
    if (functions.TryGetValue(name, out List<AbstractFunctionDeclaration>? overloads))
    {
      foreach (AbstractFunctionDeclaration f in overloads)
      {
        if (f.Parameters.Select(p => p.ResultType).SequenceEqual(argumentTypes))
        {
          return f;
        }
      }
    }

    return parent?.GetFunctionDeclaration(name, argumentTypes)
      ?? throw new InvalidSymbolException(name, "function", "variable");
  }

  public AbstractTypeDeclaration GetTypeDeclaration(string name)
  {
    if (types.TryGetValue(name, out AbstractTypeDeclaration? type))
    {
      return type;
    }

    return parent?.GetTypeDeclaration(name)
      ?? throw UnknownSymbolException.UndefinedType(name);
  }

  public void DeclareVariable(AbstractVariableDeclaration symbol)
  {
    if (!variables.TryAdd(symbol.Name, symbol))
    {
      throw DuplicateSymbolException.DuplicateVariableOrFunction(symbol.Name);
    }
  }

  public void DeclareFunction(AbstractFunctionDeclaration symbol)
  {
    if (!functions.TryGetValue(symbol.Name, out List<AbstractFunctionDeclaration>? overloads))
    {
      overloads = new List<AbstractFunctionDeclaration>();
      functions[symbol.Name] = overloads;
    }

    bool duplicate = overloads.Any(f =>
      f.Parameters.Select(p => p.ResultType)
        .SequenceEqual(symbol.Parameters.Select(p => p.ResultType)));

    if (duplicate)
    {
      throw DuplicateSymbolException.DuplicateVariableOrFunction(
        $"{symbol.Name}({string.Join(",", symbol.Parameters.Select(p => p.ResultType))})");
    }

    overloads.Add(symbol);
  }

  public void DeclareType(AbstractTypeDeclaration symbol)
  {
    if (!types.TryAdd(symbol.Name, symbol))
    {
      throw DuplicateSymbolException.DuplicateType(symbol.Name);
    }
  }

  public IReadOnlyList<AbstractFunctionDeclaration> GetFunctionOverloads(string name)
  {
    if (functions.TryGetValue(name, out List<AbstractFunctionDeclaration>? overloads))
    {
      return overloads;
    }

    return parent?.GetFunctionOverloads(name) ?? Array.Empty<AbstractFunctionDeclaration>();
  }
}