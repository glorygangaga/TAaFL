using Execution;

using Runtime;

namespace Parser.UnitTests;

public class ParserTest
{
  private static readonly decimal Tolerance = (decimal)Math.Pow(0.1, 4);

  private readonly FakeEnvironment environment;

  public ParserTest()
  {
    environment = new FakeEnvironment();
  }

  [Theory]
  [MemberData(nameof(GetParseTheory))]
  [MemberData(nameof(GetFunctionCalls))]
  [MemberData(nameof(GetExamplePrograms))]
  public void ParseTest(string code, List<string> expected)
  {
    Parser parser = new(environment, code);
    parser.ParseProgram();
    IReadOnlyList<Value> actualValues = environment.Results;

    List<string> actual = actualValues.Select(v => v.ToString()).ToList();

    Assert.Equal(expected.Count, actual.Count);

    for (int i = 0; i < expected.Count; ++i)
    {
      Value actualValue = actualValues[i];

      if (actualValue.IsFloat())
      {
        decimal actualDecimal = (decimal)actualValue.AsFloat();
        decimal expectedDecimal = decimal.Parse(expected[i], System.Globalization.CultureInfo.InvariantCulture);

        if (Math.Abs(expectedDecimal - actualDecimal) > Tolerance)
        {
          Assert.Fail($"Expected does not match actual at index {i}: {expectedDecimal} != {actualDecimal}");
        }
      }
      else if (actualValue.IsInt())
      {
        int actualInt = actualValue.AsInt();
        int expectedInt = int.Parse(expected[i], System.Globalization.CultureInfo.InvariantCulture);
        Assert.Equal(expectedInt, actualInt);
      }
      else if (actualValue.IsString())
      {
        Assert.Equal(expected[i], actualValue.AsString());
      }
      else
      {
        Assert.Equal(expected[i], actualValue.ToString());
      }
    }
  }

  [Theory]
  [MemberData(nameof(GetExampleErrorPrograms))]
  public void ParseErrorTest(string code)
  {
    Parser parser = new(environment, code);
    Assert.ThrowsAny<Exception>(() => parser.ParseProgram());
  }

  public static TheoryData<string, List<string>> GetParseTheory()
  {
    return new TheoryData<string, List<string>>
        {
            { "print(1);", new List<string> { "1" } },
            { "print(10 + 5 - 2);", new List<string> { "13" } },
            { "print(-10 - 5 - -5);", new List<string> { "-10" } },
            { "print(10 * 5 / 2);", new List<string> { "25" } },
            { "print(6 // 5);", new List<string> { "1" } },
            { "print(10 % 2);", new List<string> { "0" } },
            { "print(1.128 - 8 + 7.5);", new List<string> { "0.628" } },
            { "print(2 ** 5);", new List<string> { "32" } },
            { "print((2 + 3) / 10);", new List<string> { "0.5" } },
            { "print((-2) ** 10);", new List<string> { "1024" } },
            { "print(-2 ** 10);", new List<string> { "-1024" } },
            { "print(--5);", new List<string> { "4" } },
            { "print(++5);", new List<string> { "6" } },
            { "print(5--);", new List<string> { "4" } },
            { "print(5++);", new List<string> { "6" } },
            { "print(Pi * 2);", new List<string> { "6.28318530716" } },
            { "print(not not true);", new List<string> { "True" } },
            { "print(not 5);", new List<string> { "False" } },
            { "print(5 > 1 and 4 >= 4);", new List<string> { "True" } },
            { "print(1 < 0 or 13 <= 3);", new List<string> { "False" } },
            { "print(312 != 31);", new List<string> { "True" } },
        };
  }

  public static TheoryData<string, List<string>> GetFunctionCalls()
  {
    return new TheoryData<string, List<string>>
        {
            { "print(min(5, 4));", new List<string> { "4" } },
            { "print(max(5, 4));", new List<string> { "5" } },
            { "print(abs(-15));", new List<string> { "15" } },
            { "print(pow(2, 10));", new List<string> { "1024" } },
            { "print(ceil(1.4));", new List<string> { "2" } },
            { "print(floor(1.6));", new List<string> { "1" } },
            { "print(round(1.4));", new List<string> { "1" } },
            { "print(min(max(1, 5), min(10, 6)));", new List<string> { "5" } },
            { "print(length(\"Hello\"));", new List<string> { "5" } },
            { "print(substring(\"Hello\", 1, 3));", new List<string> { "ell" } },
            { "print(contains(\"Hello\", \"ell\"));", new List<string> { "True" } },
            { "print(startsWith(\"Hello\", \"He\"));", new List<string> { "True" } },
            { "print(endsWith(\"Hello\", \"lo\"));", new List<string> { "True" } },
            { "print(toLower(\"Hello\"));", new List<string> { "hello" } },
            { "print(toUpper(\"Hello\"));", new List<string> { "HELLO" } },
            { "print(trim(\"  Hello  \"));", new List<string> { "Hello" } },
            { "print(indexOf(\"Hello\", \"l\"));", new List<string> { "2" } },
            { "print(lastIndexOf(\"Hello\", \"l\"));", new List<string> { "3" } },
            { "print(lastIndexOf(\"Hello\", \"l\"));", new List<string> { "3" } },
            { "print(toString(42));", new List<string> { "42" } },
            { "print(toString(3.14));", new List<string> { "3.14" } },
            { "print(toString(true));", new List<string> { "True" } },
            { "print(toInt(\"123\"));", new List<string> { "123" } },
            { "print(toInt(3.99));", new List<string> { "3" } },
            { "print(toFloat(\"3.14\"));", new List<string> { "3.14" } },
            { "print(toFloat(5));", new List<string> { "5" } },
            { "print(toBool(\"true\"));", new List<string> { "True" } },
            { "print(toBool(1));", new List<string> { "True" } },
            { "print(toBool(0));", new List<string> { "False" } },
            { "print(isInt(\"123\"));", new List<string> { "False" } },
            { "print(isInt(123));", new List<string> { "True" } },
            { "print(isFloat(3.14));", new List<string> { "True" } },
            { "print(isBool(true));", new List<string> { "True" } },
            { "print(isStr(\"text\"));", new List<string> { "True" } },
            { "print(\"Hello\");", new List<string> { "Hello" } },
            { "print(\"Hello\" + \" world\");", new List<string> { "Hello world" } },
        };
  }

  public static TheoryData<string, List<string>> GetExamplePrograms()
  {
    return new TheoryData<string, List<string>>
        {
            {
                @"
                func main:void()
                {
                  let x: int = 1;
                  const y: int = 10;
                  print(x + y);
                }
                ", new List<string> { "11" }
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
                ", new List<string> { "200" }
            },
            {
                @"
                func main:void()
                {
                  const _r: int = 10;
                  print(Pi * pow(_r, 2));
                }
                ", new List<string> { "314.15927" }
            },
            {
              @"
              func nothing:int(a:int, b:int)
              {
                return a + b;
              }
              
              func main:void()
              {
                nothing(1, 1);
              }
              ", new List<string> { }
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
          "const element: void = 0;",
          "const i: str = 1.0;",
          "func nothing:int() {} nothing();",
          "func nothing:int(a: str) {} nothing(1);",
          "func nothing:int() { return 1.4; } nothing();",
        };
  }
}
