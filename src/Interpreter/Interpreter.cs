using Ast.Expressions;

using Execution;

using Semantics;

namespace Interpreter;

public class Interpreter
{
  private readonly Context context;
  private readonly IEnvironment environment;
  private readonly Builtins builtins;

  public Interpreter(Context newContext, IEnvironment newEnvironment)
  {
    context = newContext;
    environment = newEnvironment;
    builtins = new Builtins();
  }

  public void Execute(string code)
  {
    if (string.IsNullOrEmpty(code))
    {
      throw new ArgumentException("Source code cannot be null or empty", nameof(code));
    }

    Parser.Parser parser = new(environment, code);
    Expression program = parser.ParseProgram();

    SemanticsChecker checker = new(builtins.Functions, builtins.Types);
    checker.Check(program);

    AstEvaluator evaluator = new AstEvaluator(context);
    evaluator.Evaluate(program);
  }
}
