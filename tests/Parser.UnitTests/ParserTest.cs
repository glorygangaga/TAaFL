using Execution;

namespace Parser.UnitTests;

public class ParserTest
{
  private readonly Context context;
  private readonly FakeEnvironment environment;

  public ParserTest()
  {
    environment = new FakeEnvironment();
    context = new Context(environment);
  }

  [Theory]
  [MemberData(nameof(GetParseTheory))]
  [MemberData(nameof(GetFunctionCalls))]
  [MemberData(nameof(GetExamplePrograms))]
  public void ParseTest(string code, List<decimal> expected)
  {
    Parser parser = new(context, environment, code);
    parser.ParseProgram();
    IReadOnlyList<decimal> actual = environment.Results;

    for (int i = 0, iMax = Math.Min(expected.Count, actual.Count); i < iMax; ++i)
    {
      Assert.Equal(expected[i], actual[i]);
    }

    if (expected.Count != actual.Count)
    {
      Assert.Fail(
          $"Actual results count does not match expected. Expected: {expected.Count}, Actual: {actual.Count}."
      );
    }
  }

  [Theory]
  [MemberData(nameof(GetExampleErrorPrograms))]
  public void ParseErrorTest(string code)
  {
    Parser parser = new(context, environment, code);

    Assert.Throws<UnexpectedLexemeException>(parser.ParseProgram);
  }

  public static TheoryData<string, List<decimal>> GetParseTheory()
  {
    return new TheoryData<string, List<decimal>>
    {
      { "print(1);", [1] },
      { "print(10 + 5 - 2);", [13m] },
      { "print(-10 - 5 - -5);", [-10m] },
      { "print(10 * 5 / 2);", [25m] },
      { "print(6 // 5);", [1m] },
      { "print(10 % 2);", [0] },
      { "print(1.128 - 8 + 7.5);", [0.628m] },
      { "print(2 ** 5);", [32m] },
      { "print(10 % 3);", [1] },
      { "print((2 + 3) / 10);", [0.5m] },
      { "print((-2) ** 10);", [1024] },
      { "print(-2 ** 10);", [-1024] },
      { "print(--5);", [4] },
      { "print(++5);", [6] },
      { "print(5--);", [4] },
      { "print(5++);", [6] },
      { "print(Pi * 2);", [6.28318530716m] },
      { "print(not not true);", [1m] },
      { "print(not 5);", [0] },
      { "print(not true == false);", [1m] },
      { "print(3 + 4 * 5 == 3 + (4 * 5));", [1m] },
      { "print(5 > 1 and 4 >= 4);", [1] },
      { "print(1 < 0.3 or 13 <= 3);", [0] },
      { "print(312 != 31);", [1] },
      { "print(false == 0);", [1] },
      { "print(true == 1);", [1] },
      { "print(2 + 3 * 4 ** 2);", [50m] },
      { "print(2 ** 3 ** 2);", [512] },
      { "print((2 ** 3) ** 2);", [64m] },
      { "print(10 / 2 * 5 + 3);", [28m] },
      { "print(5 == 5.1);", [0m] },
      { "print(1 and 2 or 0);", [1] },
      { "print((10 - 10) or (2 * 2));", [1] },
      { "print(10 > 0 ? 10 : 0);", [10] },
    };
  }

  public static TheoryData<string, List<decimal>> GetFunctionCalls()
  {
    return new TheoryData<string, List<decimal>>
    {
      { "print(min(5, 4));", [4] },
      { "print(max(5, 4));", [5] },
      { "print(abs(-15));", [15] },
      { "print(pow(2, 10));", [1024] },
      { "print(ceil(1.4));", [2] },
      { "print(floor(1.6));", [1] },
      { "print(round(1.4));", [1] },
      { "print(min(max(1, 5), min(10, 6)));", [5m] },
      { "print(min(1, 2, 3, 4, 5));", [1m] },
      { "print(max(10, 9, 8, 7, 6));", [10] },
      { "print(pow(2, pow(2, 3)));", [256m] },
    };
  }

  public static TheoryData<string, List<decimal>> GetExamplePrograms()
  {
    return new TheoryData<string, List<decimal>>
    {
      {
        @"
        func main:void()
        {
          let x: int = 1;
          const y: int = 10;
          print(x + y);
        }
        ", [11]
      },
      {
        @"
        func main:void()
        {
          let t: int;
          t = 20;
          const result: int = t * 10;
          print(max(result, 199));
        }
        ", [200]
      },
      {
        @"
        func main:void()
        {
          const _r: int = 10;
          print(Pi * pow(_r, 2));
        }
        ", [314.159265358m]
      },
      {
        @"
        func main:void()
        {
          const x: int = 2;
          const y: int = 2;
          const result: int = (x + y) ** 0.5;
          print(result);
        }
        ", [2]
      },
    };
  }

  public static TheoryData<string> GetExampleErrorPrograms()
  {
    return new TheoryData<string>
    {
      "let value: int = 0",
      "const max: int = 0;",
      "const value: int;",
    };
  }
}