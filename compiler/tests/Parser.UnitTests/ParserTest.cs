using Execution;

namespace Parser.UnitTests;

public class ParserTest
{
  private readonly Context context;
  private readonly FakeEnvironment environment;

  public ParserTest()
  {
    context = new Context();
    environment = new FakeEnvironment();
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
      { "1;", [1] },
      { "10 + 5 - 2;", [13m] },
      { "-10 - 5 - -5;", [-10m] },
      { "10 * 5 / 2;", [25m] },
      { "6 // 5;", [1m] },
      { "10 % 2;", [0] },
      { "1.128 - 8 + 7.5;", [0.628m] },
      { "2 ** 5;", [32m] },
      { "10 % 3;", [1] },
      { "(2 + 3) / 10;", [0.5m] },
      { "(-2) ** 10;", [1024] },
      { "-2 ** 10;", [-1024] },
      { "--5;", [4] },
      { "++5;", [6] },
      { "5--;", [4] },
      { "5++;", [6] },
      { "Pi * 2;", [6.28318530716m] },
      { "not not true;", [1m] },
      { "not 5;", [0] },
      { "not true == false;", [1m] },
      { "3 + 4 * 5 == 3 + (4 * 5);", [1m] },
      { "5 > 1 and 4 >= 4;", [1] },
      { "1 < 0.3 or 13 <= 3;", [0] },
      { "312 != 31;", [1] },
      { "false == 0;", [1] },
      { "true == 1;", [1] },
      { "2 + 3 * 4 ** 2;", [50m] },
      { "2 ** 3 ** 2;", [512] },
      { "(2 ** 3) ** 2;", [64m] },
      { "10 / 2 * 5 + 3;", [28m] },
      { "5 == 5.1;", [0m] },
      { "1 and 2 or 0;", [1] },
      { "(10 - 10) or (2 * 2);", [1] },
      { "10 > 0 ? 10 : 0;", [10] },
    };
  }

  public static TheoryData<string, List<decimal>> GetFunctionCalls()
  {
    return new TheoryData<string, List<decimal>>
    {
      { "min(5, 4);", [4] },
      { "max(5, 4);", [5] },
      { "abs(-15);", [15] },
      { "pow(2, 10);", [1024] },
      { "ceil(1.4);", [2] },
      { "floor(1.6);", [1] },
      { "round(1.4);", [1] },
      { "min(max(1, 5), min(10, 6));", [5m] },
      { "min(1, 2, 3, 4, 5);", [1m] },
      { "max(10, 9, 8, 7, 6);", [10] },
      { "pow(2, pow(2, 3));", [256m] },
    };
  }

  public static TheoryData<string, List<decimal>> GetExamplePrograms()
  {
    return new TheoryData<string, List<decimal>>
    {
      {
        @"let x: int = 1;
          const y: int = 10;
          let t: int;
          print(x + y);
        ", [11]
      },
      {
        @"let t: int;
        t = 20;
        const result: int = t * 10;
        print(max(result, 199));
        ", [200]
      },
      {
        @"const _r: int = 10;
        print(Pi * pow(_r, 2));
        ", [314.159265358m]
      },
      {
        @"const x: int = 2;
        const y: int = 2;
        const result: int = (x + y) ** 0.5;
        print(result);
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