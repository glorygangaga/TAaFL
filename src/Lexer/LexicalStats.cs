using System.Text;

namespace Lexer;

public struct LexicalData
{
  public int Keywords { get; set; }

  public int Identifiers { get; set; }

  public int NumberLiterals { get; set; }

  public int StringLiterals { get; set; }

  public int Operators { get; set; }

  public int OtherLexemes { get; set; }

  public override readonly string ToString()
  {
    return $"""
        keywords: {Keywords}
        identifier: {Identifiers}
        number literals: {NumberLiterals}
        string literals: {StringLiterals}
        operators: {Operators}
        other lexemes: {OtherLexemes}
      """;
  }
}

public static class LexicalStats
{
  private static readonly HashSet<TokenType> Keywords = new()
  {
    TokenType.Const, TokenType.Let, TokenType.Str, TokenType.Int, TokenType.Bool, TokenType.Float, TokenType.Void, TokenType.True,
    TokenType.False, TokenType.Not, TokenType.And, TokenType.Or, TokenType.If, TokenType.Else, TokenType.For, TokenType.While,
    TokenType.Func, TokenType.Return, TokenType.Import, TokenType.Input, TokenType.Print,
  };

  private static readonly HashSet<TokenType> Operators = new()
  {
    TokenType.Equal, TokenType.NotEqual, TokenType.LessThan, TokenType.LessThanOrEqual, TokenType.GreaterThan, TokenType.GreaterThanOrEqual,
    TokenType.Plus, TokenType.Increment, TokenType.Decrement, TokenType.Minus, TokenType.Multiplication, TokenType.Division,
    TokenType.Remainder, TokenType.Exponentiation, TokenType.And, TokenType.Not, TokenType.Or, TokenType.Assignment,
    TokenType.ColonTypeIndication,
    TokenType.OpenParenthesis, TokenType.CloseParenthesis,
  };

  public static string CollectFromFile(string path)
  {
    string text = File.ReadAllText(path, Encoding.UTF8);
    Lexer lexer = new(text);

    LexicalData lexemData = new();
    for (Token t = lexer.ParseToken(); t.Type != TokenType.EndOfFile; t = lexer.ParseToken())
    {
      if (t.Type == TokenType.Identifier)
      {
        lexemData.Identifiers++;
      }
      else if (t.Type == TokenType.NumberLiteral)
      {
        lexemData.NumberLiterals++;
      }
      else if (t.Type == TokenType.StringLiteral)
      {
        lexemData.StringLiterals++;
      }
      else if (Keywords.Contains(t.Type))
      {
        lexemData.Keywords++;
      }
      else if (Operators.Contains(t.Type))
      {
        lexemData.Operators++;
      }
      else
      {
        lexemData.OtherLexemes++;
      }
    }

    return lexemData.ToString();
  }
}