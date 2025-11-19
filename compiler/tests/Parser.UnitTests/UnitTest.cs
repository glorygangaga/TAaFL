namespace Parser.UnitTests;

public class ParserTest
{
  [Theory]
  [MemberData(nameof(GetParseTheory))]
  public void ParseTest(string code, decimal[] expected)
  {
    List<decimal> actual = Parser.ExecuteExpr(code);
    for (int i = 0; i < expected.Length; i++)
    {
      Assert.Equal(expected[i], actual[i]);
    }
  }

  public static TheoryData<string, decimal[]> GetParseTheory()
  {
    return new TheoryData<string, decimal[]>
    {
      { "1, 2", [1, 2] },
      { "10 + 5 - 2", [13m] },
      { "-10 - 5 - -5", [-10m] },
      { "10 * 5 / 2", [25m] },
      { "6 // 5", [1m] },
      { "10 % 2", [0] },
      { "1.128 - 8 + 7.5", [0.628m] },
      { "2 ** 5, 10 % 3", [32m, 1m] },
      { "(2 + 3) / 10", [0.5m] },
      { "(-2) ** 10, -2 ** 10", [1024, -1024] },
      { "--5, ++5, 5--, 5++", [4, 6, 4, 6] },
      { "min(5, 4), max(5, 4), abs(-15), pow(2, 10)", [4, 5, 15, 1024] },
      { "round(1.4), ceil(1.4), floor(1.6)", [1, 2, 1] },
      { "Pi * 2, Euler * 3", [6.28318530716m, 8.15484548538m] },
      { "not not true", [1m] },
      { "not 5, not 0, not 1", [0, 1, 0] },
      { "min(max(1, 5), min(10, 6))", [5m] },
      { "min(1, 2, 3, 4, 5), max(10, 9, 8, 7, 6)", [1m, 10m] },
      { "pow(2, pow(2, 3))", [256m] },
      { "not true == false", [1m] },
      { "3 + 4 * 5 == 3 + (4 * 5)", [1m] },
      { "-+5, +-5, --5", [-5m, -5m, 4m] },
      { "5 > 1 and 4 >= 4, 1 < 0.3 or 13 <= 3", [1, 0] },
      { "312 != 31", [1] },
      { "false == 0, true == 1", [1, 1] },
      { "2 + 3 * 4 ** 2", [50m] },
      { "10 / 2 * 5 + 3", [28m] },
      { "5 == 5.0, 5 == 5.1", [1m, 0m] },
    };
  }
}