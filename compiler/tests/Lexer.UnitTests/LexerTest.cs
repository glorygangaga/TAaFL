using Xunit;

namespace Lexer.UnitTests;

public class LexerTest
{
  [Theory]
  [MemberData(nameof(GetData))]
  public void LexerTestTheory(string text, List<string> expected)
  {
    Console.WriteLine(text);
    Assert.Equal(expected, [""]);
  }

  public static TheoryData<string, List<string>> GetData()
  {
    return new TheoryData<string, List<string>>
    {
      {
        "Select 2025;", [""]
      },
      {
        "SELECT first_name FROM student;", [""]
      },
      {
        "SELECT first_name, last_name, email FROM student;", [""]
      },
      {
        "select first_name, last_name FrOM student;", [""]
      },
      {
        "SELECT count + 1 FROM counter;", [""]
      },
      {
        "SELECT starts_at - ends_at + 1 FROM meeting;", [""]
      },
      {
        "SELECT radius, 2 _ 3.14159265358 _ radius FROM circle;", [""]
      },
      {
        "SELECT duration / 2 FROM phone_call;", [""]
      },
      {
        "SELECT circle WHERE radius > 10 AND radius < 25 AND NOT is_deleted;", [""]
      },
      {
        "SELECT circle WHERE radius >= 10 AND radius <= 25;", [""]
      },
      {
        "SELECT to_be OR NOT to_be;", [""]
      },
      {
        "-- ...", [""]
      },
    };
  }
}