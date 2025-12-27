using Execution;

namespace Interpreter;

public class Interpreter
{
  private readonly Context context;

  public Interpreter(Context newContext)
  {
    context = newContext;
  }

  public void Execute(string code)
  {
    if (string.IsNullOrEmpty(code))
    {
      throw new ArgumentException("Source code cannot be null or empty", nameof(code));
    }

    Parser.Parser parser = new(code);
    BlockStatement program = parser.ParseProgram();

    AstEvaluator evaluator = new AstEvaluator(context);
    evaluator.Evaluate(program);
  }
}
