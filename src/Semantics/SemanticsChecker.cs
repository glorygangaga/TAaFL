using Ast;
using Ast.Declarations;
using Ast.Expressions;

using Semantics.Passes;
using Semantics.Symbols;

namespace Semantics;

public class SemanticsChecker
{
  private readonly AbstractPass[] passes;

  public SemanticsChecker(
    IReadOnlyList<BuiltinFunction> builtinFunctions,
    IReadOnlyList<BuiltinType> builtinTypes
  )
  {
    SymbolsTable globalSymbols = new(parent: null);

    foreach (BuiltinFunction function in builtinFunctions)
    {
      globalSymbols.DeclareFunction(function);
    }

    foreach (BuiltinType type in builtinTypes)
    {
      globalSymbols.DeclareType(type);
    }

    passes =
    [
      new ResolveNamesPass(globalSymbols),
      new CheckContextSensitiveRulesPass(),
      new ResolveTypesPass(),
      new CheckTypesPass(),
    ];
  }

  public void Check(BlockStatement program)
  {
    foreach (AbstractPass pass in passes)
    {
      program.Accept(pass);
    }
  }
}
