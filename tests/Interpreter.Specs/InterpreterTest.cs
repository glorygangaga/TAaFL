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

    FakeEnvironment environment = new FakeEnvironment();
    Context context = new Context(environment);

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
        @"
        func main:void()
        {
          for (let i:int = 0; i < 5; i++)
          {
            print(i);
          }
        }
        ",
        new Tuple<List<decimal>, List<decimal>>(
          new List<decimal> { },
          new List<decimal> { 0, 1, 2, 3, 4 }
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
        new Tuple<List<decimal>, List<decimal>>(
          new List<decimal> { 1 },
          new List<decimal> { 100 }
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
        new Tuple<List<decimal>, List<decimal>>(
          new List<decimal> { },
          new List<decimal> { 0, 1, 2, 3, 4 }
        )
      },
      {
        @"
          func main:void()
          {
            let radius:int = input();
            let area:int = Pi * radius ** 2;
            print(area);
          }
        ",
        new Tuple<List<decimal>, List<decimal>>(
          new List<decimal> { 10 },
          new List<decimal> { 314.159265358m }
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

          let x1:int = (- b + d ** 0.5) / (2 * a);
          let x2:int = (- b - d ** 0.5) / (2 * a);
          print(x1);
          print(x2);
        }
        ",
        new Tuple<List<decimal>, List<decimal>>(
          new List<decimal> { 1, -5, 4 },
          new List<decimal> { 4, 1 }
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
        new Tuple<List<decimal>, List<decimal>>(
          new List<decimal> { 7 },
          new List<decimal> { 13 }
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
        new Tuple<List<decimal>, List<decimal>>(
          new List<decimal> { 30 },
          new List<decimal> { 0 }
        )
      },
      {
        @"
        func calculate_discriminant:int(a:int, b:int, c:int)
        {
            return b**2 - 4 * a * c;
        }

        func nothing:void()
        {
          return;
          print(1);
        }

        func main:void()
        {
            nothing();
            let a:int = input();
            let b:int = input();
            let c:int = input();

            if (a == 0) 
            {
                if (b == 0) 
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
                let d:int = calculate_discriminant(a, b, c);

                if (d < 0)
                {
                    print(0);
                }
                elif (d == 0)
                {
                    let x:int = - b / (2 * a); 
                    /// x можно заменить на x1 для для проверки области видимости
                    print(1, x);
                }
                else
                {
                    let x1:int = (- b + d ** 0.5) / (2 * a);
                    let x2:int = (- b - d ** 0.5) / (2 * a);
                    /// возведение в степень можно заменить на встроенную функцию pow(d, 0,5)
                    print(2, x1, x2)
                }
            }
        }
        ",
        new Tuple<List<decimal>, List<decimal>>(
          new List<decimal> { 1, 2, 1 },
          new List<decimal> { 1, -1 }
        )
      },
      {
        @"
          func main:void()
          {
            for (let i: int = 0; i < 10; i++)
            {
              if (i % 2 == 0)
              {
                if (i == 2)
                {
                  continue;
                }

                print(i);
              }

              if (i == 7)
              {
                break;
              }
            }
          }
        ",
        new Tuple<List<decimal>, List<decimal>>(
          new List<decimal> { },
          new List<decimal> { 0, 4, 6 }
        )
      },
    };
  }
}