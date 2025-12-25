using Ast.Declarations;
using Ast.Expressions;

using Semantics.Exceptions;
using Semantics.Helpers;
using Semantics.Symbols;

using ValueType = Runtime.ValueType;

namespace Semantics.Passes;

/// <summary>
/// Проход по AST, устанавливающий соответствие имён и символов (объявлений).
/// </summary>
public sealed class ResolveNamesPass : AbstractPass
{
  /// <summary>
  /// В таблицу символов складываются объявления.
  /// </summary>
  private SymbolsTable symbols;

  public ResolveNamesPass(SymbolsTable globalSymbols)
  {
    symbols = globalSymbols;
  }

  public override void Visit(FunctionCallExpression e)
  {
    base.Visit(e);

    IReadOnlyList<AbstractFunctionDeclaration> overloads = symbols.GetFunctionOverloads(e.Name);

    if (overloads.Count == 0)
    {
      throw new InvalidFunctionCallException($"Function '{e.Name}' not found");
    }

    e.FunctionOverloads = overloads;
  }

  public override void Visit(VariableExpression e)
  {
    base.Visit(e);
    e.Variable = symbols.GetVariableDeclaration(e.Name);
  }

  public override void Visit(VariableDeclaration d)
  {
    base.Visit(d);
    d.DeclaredType = d.DeclaredTypeName != null ? symbols.GetTypeDeclaration(d.DeclaredTypeName) : null;
    symbols.DeclareVariable(d);
  }

  public override void Visit(ConstantDeclaration d)
  {
    base.Visit(d);
    d.DeclaredType = symbols.GetTypeDeclaration(d.DeclaredTypeName);
    symbols.DeclareVariable(d);
  }

  public override void Visit(FunctionDeclaration d)
  {
    d.ResultType = d.DeclaredTypeName != null ? symbols.GetTypeDeclaration(d.DeclaredTypeName).ResultType : symbols.GetTypeDeclaration("void").ResultType;
    d.DeclaredType = d.DeclaredTypeName != null ? symbols.GetTypeDeclaration(d.DeclaredTypeName) : null;

    symbols.DeclareFunction(d);
    symbols = new SymbolsTable(symbols);

    try
    {
      base.Visit(d);
    }
    finally
    {
      symbols = symbols.Parent!;
    }
  }

  public override void Visit(ForLoopExpression e)
  {
    // Создаём дочернюю таблицу символов.
    symbols = new SymbolsTable(symbols);
    try
    {
      VariableDeclaration var = new VariableDeclaration(e.IteratorName, "int", e.StartValue);
      symbols.DeclareVariable(var);
      base.Visit(e);
    }
    finally
    {
      symbols = symbols.Parent!;
    }
  }

  public override void Visit(ParameterDeclaration d)
  {
    base.Visit(d);
    d.Type = symbols.GetTypeDeclaration(d.TypeName);
    symbols.DeclareVariable(d);
  }

  public override void Visit(IfElseExpression e)
  {
    e.Condition.Accept(this);

    symbols = new SymbolsTable(symbols);
    try
    {
      e.ThenBranch.Accept(this);
    }
    finally
    {
      symbols = symbols.Parent!;
    }

    if (e.ElseBranch != null)
    {
      symbols = new SymbolsTable(symbols);
      try
      {
        e.ElseBranch.Accept(this);
      }
      finally
      {
        symbols = symbols.Parent!;
      }
    }
  }
}