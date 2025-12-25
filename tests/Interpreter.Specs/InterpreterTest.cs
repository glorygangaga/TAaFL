using Execution;

using Parser;

using Runtime;

namespace Interpreter.Specs;

public class InterpreterTest
{
  private static readonly decimal Tolerance = (decimal)Math.Pow(0.1, 4);

  [Theory]
  [MemberData(nameof(GetExamplePrograms))]
  public void ParseTest(string source, Tuple<List<Value>, List<string>> tuple)
  {
    List<Value> inputValues = tuple.Item1;
    List<string> expected = tuple.Item2;

    FakeEnvironment environment = new FakeEnvironment();
    Context context = new Context(environment);

    for (int i = inputValues.Count - 1; i >= 0; i--)
    {
      environment.Write(inputValues[i]);
    }

    Interpreter interpreter = new Interpreter(context, environment);
    interpreter.Execute(source);

    IReadOnlyList<Value> actualValues = environment.Results;

    Assert.Equal(expected.Count, actualValues.Count);

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
  [MemberData(nameof(GetParseTheory))]
  [MemberData(nameof(GetFunctionCalls))]
  [MemberData(nameof(GetExampleSimplePrograms))]
  public void ParseSimpleTest(string code, List<string> expected)
  {
    FakeEnvironment environment = new FakeEnvironment();
    Context context = new Context(environment);
    Interpreter interpreter = new Interpreter(context, environment);
    interpreter.Execute(code);

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
    FakeEnvironment environment = new FakeEnvironment();
    Context context = new Context(environment);
    Interpreter interpreter = new Interpreter(context, environment);
    Assert.ThrowsAny<Exception>(() => interpreter.Execute(code));
  }

  public static TheoryData<string, Tuple<List<Value>, List<string>>> GetExamplePrograms()
  {
    return new TheoryData<string, Tuple<List<Value>, List<string>>>
    {
      {
        @"
        func main:void()
        {
          for (let i:int = 0; i < 6; i++)
          {
            print(i);
          }
        }
        ",
        new Tuple<List<Value>, List<string>>(
          new List<Value> { },
          new List<string> { "0", "1", "2", "3", "4", "5" }
        )
      },
      {
        @"
        func main:void()
        {
          const i: int = input();
          if (i == 0)
          {
            print(i);
          }
          elif (i == 1)
          {
            print(i * 100);
          } 
          else
          {
            print(i * 1000);
          }
        }
        ",
        new Tuple<List<Value>, List<string>>(
          new List<Value> { new(1) },
          new List<string> { "100" }
        )
      },
      {
        @"
        func main:void()
        {
          let i: int = 0;
          while (i < 5)
          {
            print(i);
            i = i + 1;
          }
        }
        ",
        new Tuple<List<Value>, List<string>>(
          new List<Value> { },
          new List<string> { "0", "1", "2", "3", "4" }
        )
      },
      {
        @"
        func main:void()
        {
          let radius:int = input();
          let area:float = Pi * radius ** 2;
          print(area);
        }
        ",
        new Tuple<List<Value>, List<string>>(
          new List<Value> { new(10) },
          new List<string> { "314.159265358" }
        )
      },
      {
        @"
        func main:void()
        {
          let a:int = input();
          let b:int = input();
          let c:int = input();

          let d:int = b ** 2 - 4 * a * c;

          let x1:float = (- b + d ** 0.5) / (2 * a);
          let x2:float = (- b - d ** 0.5) / (2 * a);
          print(x1);
          print(x2);
        }
        ",
        new Tuple<List<Value>, List<string>>(
          new List<Value> { new(1), new(-5), new(4) },
          new List<string> { "4", "1" }
        )
      },
      {
        @"
        func fibonacci:int(n:int)
        {
          if (n <= 1)
          {
            return n;
          }
          return fibonacci(n - 1) + fibonacci(n - 2);
        }

        func main:void()
        {
          let n:int = input();
          print(fibonacci(n));
        }
        ",
        new Tuple<List<Value>, List<string>>(
          new List<Value> { new(7) },
          new List<string> { "13" }
        )
      },
      {
        @"
        func calculate_discriminant:float(a:float, b:float, c:float)
        {
          return b ** 2.0 - 4.0 * a * c;
        }

        func main:void()
        {
            let a:float = input();
            let b:float = input();
            let c:float = input();

            if (a == 0.0) 
            {
                if (b == 0.0) 
                {
                    print(0);
                } 
                else 
                { 
                    print(1, - c / b);
                }
            } 
            else 
            {
                let d:float = calculate_discriminant(a, b, c);

                if (d < 0.0)
                {
                  print(0);
                }
                elif (d == 0.0)
                {
                  let x:float = - b / (2 * a); 
                  /// x можно заменить на x1 для для проверки области видимости
                  print(1, x);
                }
                else
                {
                  let x1:float = (- b + d ** 0.5) / (2 * a);
                  let x2:float = (- b - d ** 0.5) / (2 * a);
                  /// возведение в степень можно заменить на встроенную функцию pow(d, 0,5)
                  print(2, x1, x2)
                }
            }
        }
        ",
        new Tuple<List<Value>, List<string>>(
          new List<Value> { new(1f), new(-3f), new(2f) },
          new List<string> { "2", "2", "1" }
        )
      },
    };
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
        { "print((2 + 3) / 10);", new List<string> { "0" } },
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

  public static TheoryData<string, List<string>> GetExampleSimplePrograms()
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
                  let t: int = 20;
                  const result: int = t * 10;
                  print(max(result, 199));
                }
                ", new List<string> { "200" }
            },
            {
                @"
                func main:void()
                {
                  const _r: float = 10.0;
                  print(Pi * pow(_r, 2.0));
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
