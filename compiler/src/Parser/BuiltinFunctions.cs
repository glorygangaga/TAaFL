using Lexer;

namespace Parser;

public class BuiltinFunctions
{
  private readonly Dictionary<TokenType, Func<List<decimal>, decimal>> functions = new()
  {
    { TokenType.Abs, Abs },
    { TokenType.Min, Min },
    { TokenType.Max, Max },
    { TokenType.Pow, Pow },
    { TokenType.Round, Round },
    { TokenType.Ceil, Ceil },
    { TokenType.Floor, Floor },
  };

  private static readonly BuiltinFunctions InstanceValue = new();

  public static BuiltinFunctions Instance => InstanceValue;

  public decimal Invoke(TokenType token, List<decimal> arguments)
  {
    if (!functions.TryGetValue(token, out Func<List<decimal>, decimal>? function))
    {
      throw new ArgumentException($"Unknown builtin function {token}");
    }

    return function(arguments);
  }

  private static decimal Min(List<decimal> arguments)
  {
    return arguments.Min();
  }

  private static decimal Max(List<decimal> arguments)
  {
    return arguments.Max();
  }

  private static decimal Pow(List<decimal> arguments)
  {
    if (arguments.Count != 2)
    {
      throw new ArgumentException($"In pow need to 2 argments, but get {arguments.Count}");
    }

    return (decimal)Math.Pow((double)arguments[0], (double)arguments[1]);
  }

  private static decimal Ceil(List<decimal> arguments)
  {
    if (arguments.Count != 1)
    {
      throw new ArgumentException($"In pow need to 1 argments, but get {arguments.Count}");
    }

    return Math.Ceiling(arguments[0]);
  }

  private static decimal Round(List<decimal> arguments)
  {
    if (arguments.Count != 1)
    {
      throw new ArgumentException($"In pow need to 1 argments, but get {arguments.Count}");
    }

    return Math.Round(arguments[0]);
  }

  private static decimal Floor(List<decimal> arguments)
  {
    if (arguments.Count != 1)
    {
      throw new ArgumentException($"In pow need to 1 argments, but get {arguments.Count}");
    }

    return Math.Floor(arguments[0]);
  }

  private static decimal Abs(List<decimal> arguments)
  {
    if (arguments.Count != 1)
    {
      throw new ArgumentException($"In pow need to 1 argments, but get {arguments.Count}");
    }

    return Math.Abs(arguments[0]);
  }
}