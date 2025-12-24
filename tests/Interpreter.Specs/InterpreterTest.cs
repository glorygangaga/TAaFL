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

  public static TheoryData<string, Tuple<List<Value>, List<string>>> GetExamplePrograms()
  {
    return new TheoryData<string, Tuple<List<Value>, List<string>>>
    {
      {
        @"
        func main:void()
        {
          for (let i:int = 0; i < 5; i++)
          {
            print(i);
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
        func main:void()
        {
          const i:int = input();

          switch(i)
          {
            case 1:
            {
              print(1);
              break;
            }
            case 2:
            {
              print(2);
              break;
            }
            case 3:
            {
              print(3);
              break;
            }
            default:
            {
              print(0);
            }
          }
        }
        ",
        new Tuple<List<Value>, List<string>>(
          new List<Value> { new(30) },
          new List<string> { "0" }
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
}
