using System.Globalization;

using Runtime;

namespace Execution;

public class ConsoleEnvironment : IEnvironment
{
  public void Write(Value value)
  {
    Console.WriteLine("Result: " + value.ToString());
  }

  public Value Read()
  {
    string? input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
      throw new InvalidOperationException("Input is empty");
    }

    if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
    {
      return new Value(intValue);
    }

    if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
    {
      return new Value(floatValue);
    }

    if (bool.TryParse(input, out bool boolValue))
    {
      return new Value(boolValue);
    }

    return new Value(input);
  }
}
