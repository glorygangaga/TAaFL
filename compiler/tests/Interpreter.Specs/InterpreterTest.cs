using Execution;

using Parser;

namespace Interpreter.Specs;

public class InterpreterTest
{
  private static readonly decimal Tolerance = (decimal)Math.Pow(0.1, 5);

  [Theory]
  [MemberData(nameof(GetExamplePrograms))]
  public void ParseTest(string source, Tuple<List<decimal>, List<decimal>> tuple)
  {
    List<decimal> input = tuple.Item1;
    List<decimal> expected = tuple.Item2;

    Context context = new Context();
    FakeEnvironment environment = new FakeEnvironment();

    for (int i = input.Count - 1; i >= 0; i--)
    {
      environment.WriteNumber(input[i]);
    }

    Interpreter interpreter = new Interpreter(context, environment);
    interpreter.Execute(source);

    IReadOnlyList<decimal> actual = environment.Results;

    if (expected.Count != actual.Count)
    {
      Assert.Fail(
          $"Actual results count does not match expected. Expected: {expected.Count}, Actual: {actual.Count}."
      );
    }

    for (int i = 0, iMax = Math.Min(expected.Count, actual.Count); i < iMax; ++i)
    {
      if (Math.Abs(expected[i] - actual[i]) >= Tolerance)
      {
        Assert.Fail($"Expected does not match actual at index {i}: {expected[i]} != {actual[i]}");
      }
    }
  }

  public static TheoryData<string, Tuple<List<decimal>, List<decimal>>> GetExamplePrograms()
  {
    return new TheoryData<string, Tuple<List<decimal>, List<decimal>>>
    {
      {
        @"let a: int = input();
        let b: int = input();
        print(a + b);",
        new Tuple<List<decimal>, List<decimal>>(
          new List<decimal> { 3, 7 },
          new List<decimal> { 10 }
        )
      },
      {
        @"let a: int = input();
        let b: int = input();
        const result: int = -pow(a, b);
        print(result);",
        new Tuple<List<decimal>, List<decimal>>(
          new List<decimal> { 2, 10 },
          new List<decimal> { -1024 }
        )
      },
      {
        @"const radius: int = input();
          const area: int = Pi * radius ** 2;
          print(area);
        ",
        new Tuple<List<decimal>, List<decimal>>(
          new List<decimal> { 10 },
          new List<decimal> { 314.159265358m }
        )
      },
    };
  }
}