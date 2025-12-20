using System.Globalization;

namespace Execution;

public class ConsoleEnvironment : IEnvironment
{
  public void WriteNumber(decimal value)
  {
    Console.WriteLine("Result: " + value.ToString("0.#####", CultureInfo.InvariantCulture));
  }

  public decimal ReadNumber()
  {
    string? input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
      throw new InvalidOperationException("Input is empty");
    }

    if (!decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal number))
    {
      throw new FormatException($"Invalid number format: '{input}'");
    }

    return number;
  }
}