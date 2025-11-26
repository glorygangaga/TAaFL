using Execution;

namespace Interpreter;

public class Interpreter
{
  private readonly Context context;
  private readonly IEnvironment environment;

  public Interpreter(Context newContext, IEnvironment newEnvironment)
  {
    context = newContext;
    environment = newEnvironment;
  }

  public void Execute(string code)
  {
    if (string.IsNullOrEmpty(code))
    {
      throw new ArgumentException("Source code cannot be null or empty", nameof(code));
    }

    Parser.Parser parser = new(context, environment, code);
    parser.ParseProgram();
  }
}
