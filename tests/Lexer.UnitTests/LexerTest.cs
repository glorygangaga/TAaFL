namespace Lexer.UnitTests;

public class LexerTest
{
  [Theory]
  [MemberData(nameof(GetTokenizeData))]
  public void LexerTestTheory(string text, List<Token> expected)
  {
    List<Token> actual = Tokenize(text);
    Assert.Equal(expected, actual);
  }

  public static TheoryData<string, List<Token>> GetTokenizeData()
  {
    return new TheoryData<string, List<Token>>
    {
      {
        "let name: str = \"Alice\";", [
          new Token(TokenType.Let),
          new Token(TokenType.Identifier, new TokenValue("name")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Str),
          new Token(TokenType.Assignment),
          new Token(TokenType.StringLiteral, new TokenValue("Alice")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const name: str = \"Alice\";", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("name")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Str),
          new Token(TokenType.Assignment),
          new Token(TokenType.StringLiteral, new TokenValue("Alice")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const userData:str = input();", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("userData")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Str),
          new Token(TokenType.Assignment),
          new Token(TokenType.Input),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "print(\"Hello world\");", [
          new Token(TokenType.Print),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.StringLiteral, new TokenValue("Hello world")),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "func nothing:void() {}", [
          new Token(TokenType.Func),
          new Token(TokenType.Identifier, new TokenValue("nothing")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Void),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.OpenCurlyBrace),
          new Token(TokenType.CloseCurlyBracket)
        ]
      },
      {
        "func greet:str(user:str) { return user; }", [
          new Token(TokenType.Func),
          new Token(TokenType.Identifier, new TokenValue("greet")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Str),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Identifier, new TokenValue("user")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Str),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.OpenCurlyBrace),
          new Token(TokenType.Return),
          new Token(TokenType.Identifier, new TokenValue("user")),
          new Token(TokenType.Semicolon),
          new Token(TokenType.CloseCurlyBracket)
        ]
      },
      {
        "import std;", [
          new Token(TokenType.Import),
          new Token(TokenType.Identifier, new TokenValue("std")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "/// Это комментарий", [
          new Token(TokenType.SingleLineComment)
        ]
      },
      {
        "let x:int = 5; /// Комментарий после кода", [
          new Token(TokenType.Let),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Int),
          new Token(TokenType.Assignment),
          new Token(TokenType.NumberLiteral, new TokenValue("5")),
          new Token(TokenType.Semicolon),
          new Token(TokenType.SingleLineComment)
        ]
      },
      {
        @"/*
         Это многострочный комментарий
        */", [
          new Token(TokenType.OpenBlockComments),
          new Token(TokenType.CloseBlockComments)
        ]
      },
      {
        "const string:str = \"text\";", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("string")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Str),
          new Token(TokenType.Assignment),
          new Token(TokenType.StringLiteral, new TokenValue("text")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const number:int = 11;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("number")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Int),
          new Token(TokenType.Assignment),
          new Token(TokenType.NumberLiteral, new TokenValue("11")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const number:float = 1.1;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("number")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Float),
          new Token(TokenType.Assignment),
          new Token(TokenType.NumberLiteral, new TokenValue("1.1")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const isNumber:bool = false;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("isNumber")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Bool),
          new Token(TokenType.Assignment),
          new Token(TokenType.False),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const value:int = x + y - z;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("value")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Int),
          new Token(TokenType.Assignment),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.Plus),
          new Token(TokenType.Identifier, new TokenValue("y")),
          new Token(TokenType.Minus),
          new Token(TokenType.Identifier, new TokenValue("z")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const area:float = PI * R ** 2;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("area")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Float),
          new Token(TokenType.Assignment),
          new Token(TokenType.Identifier, new TokenValue("PI")),
          new Token(TokenType.Multiplication),
          new Token(TokenType.Identifier, new TokenValue("R")),
          new Token(TokenType.Exponentiation),
          new Token(TokenType.NumberLiteral, new TokenValue("2")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const value:float = 7 % 2 / 4;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("value")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Float),
          new Token(TokenType.Assignment),
          new Token(TokenType.NumberLiteral, new TokenValue("7")),
          new Token(TokenType.Remainder),
          new Token(TokenType.NumberLiteral, new TokenValue("2")),
          new Token(TokenType.Division),
          new Token(TokenType.NumberLiteral, new TokenValue("4")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const value:int = x++;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("value")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Int),
          new Token(TokenType.Assignment),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.Increment),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const value:int = x--;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("value")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Int),
          new Token(TokenType.Assignment),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.Decrement),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const value:int = ++x;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("value")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Int),
          new Token(TokenType.Assignment),
          new Token(TokenType.Increment),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.Semicolon)

        ]
      },
      {
        "const value:int = --x;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("value")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Int),
          new Token(TokenType.Assignment),
          new Token(TokenType.Decrement),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const isSame:bool = x == y;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("isSame")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Bool),
          new Token(TokenType.Assignment),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.Equal),
          new Token(TokenType.Identifier, new TokenValue("y")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const isNotSame:bool = x != y;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("isNotSame")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Bool),
          new Token(TokenType.Assignment),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.NotEqual),
          new Token(TokenType.Identifier, new TokenValue("y")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const isBigger:bool = x > y;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("isBigger")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Bool),
          new Token(TokenType.Assignment),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.GreaterThan),
          new Token(TokenType.Identifier, new TokenValue("y")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const isLess:bool = x < y;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("isLess")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Bool),
          new Token(TokenType.Assignment),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.LessThan),
          new Token(TokenType.Identifier, new TokenValue("y")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const isBiggerOrSame:bool = x >= y;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("isBiggerOrSame")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Bool),
          new Token(TokenType.Assignment),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.GreaterThanOrEqual),
          new Token(TokenType.Identifier, new TokenValue("y")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const isLessOrSame:bool = x <= y;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("isLessOrSame")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Bool),
          new Token(TokenType.Assignment),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.LessThanOrEqual),
          new Token(TokenType.Identifier, new TokenValue("y")),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "const value:bool = (x > z) and (y > z) or (z == not(w));", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("value")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Bool),
          new Token(TokenType.Assignment),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.GreaterThan),
          new Token(TokenType.Identifier, new TokenValue("z")),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.And),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Identifier, new TokenValue("y")),
          new Token(TokenType.GreaterThan),
          new Token(TokenType.Identifier, new TokenValue("z")),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.Or),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Identifier, new TokenValue("z")),
          new Token(TokenType.Equal),
          new Token(TokenType.Not),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Identifier, new TokenValue("w")),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.Semicolon)
        ]
      },
      {
        "if (x > 0) { print(x); }", [
          new Token(TokenType.If),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.GreaterThan),
          new Token(TokenType.NumberLiteral, new TokenValue("0")),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.OpenCurlyBrace),
          new Token(TokenType.Print),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.Semicolon),
          new Token(TokenType.CloseCurlyBracket)
        ]
      },
      {
        "if (x > y) { print(x); } else { print(y); }", [
          new Token(TokenType.If),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.GreaterThan),
          new Token(TokenType.Identifier, new TokenValue("y")),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.OpenCurlyBrace),
          new Token(TokenType.Print),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.Semicolon),
          new Token(TokenType.CloseCurlyBracket),
          new Token(TokenType.Else),
          new Token(TokenType.OpenCurlyBrace),
          new Token(TokenType.Print),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Identifier, new TokenValue("y")),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.Semicolon),
          new Token(TokenType.CloseCurlyBracket)
        ]
      },
      {
        "for (let i:int = 0; i < length; i++) { print(i); }", [
          new Token(TokenType.For),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Let),
          new Token(TokenType.Identifier, new TokenValue("i")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Int),
          new Token(TokenType.Assignment),
          new Token(TokenType.NumberLiteral, new TokenValue("0")),
          new Token(TokenType.Semicolon),
          new Token(TokenType.Identifier, new TokenValue("i")),
          new Token(TokenType.LessThan),
          new Token(TokenType.Identifier, new TokenValue("length")),
          new Token(TokenType.Semicolon),
          new Token(TokenType.Identifier, new TokenValue("i")),
          new Token(TokenType.Increment),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.OpenCurlyBrace),
          new Token(TokenType.Print),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Identifier, new TokenValue("i")),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.Semicolon),
          new Token(TokenType.CloseCurlyBracket)
        ]
      },
      {
        "while (true) { print(x); }", [
          new Token(TokenType.While),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.True),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.OpenCurlyBrace),
          new Token(TokenType.Print),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.Semicolon),
          new Token(TokenType.CloseCurlyBracket)
        ]
      },
      {
        "print(car);", [
          new Token(TokenType.Print),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Identifier, new TokenValue("car")),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.Semicolon),
        ]
      },
      {
        "if (x == false) { print(x); }", [
          new Token(TokenType.If),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.Equal),
          new Token(TokenType.False),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.OpenCurlyBrace),
          new Token(TokenType.Print),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.Identifier, new TokenValue("x")),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.Semicolon),
          new Token(TokenType.CloseCurlyBracket)
        ]
      },
      {
        @"const string: str = ""многострочный 
        литерал"";", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("string")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Str),
          new Token(TokenType.Assignment),
          new Token(TokenType.Error, new TokenValue("многострочный ")),
          new Token(TokenType.Identifier, new TokenValue("литерал")),
          new Token(TokenType.Error, new TokenValue(";")),
        ]
      },
      {
      @"print(""\n \t \\ \"""");", [
          new Token(TokenType.Print),
          new Token(TokenType.OpenParenthesis),
          new Token(TokenType.StringLiteral, new TokenValue("\n \t \\ \"")),
          new Token(TokenType.CloseParenthesis),
          new Token(TokenType.Semicolon),
        ]
      },
      {
        "const value:int = 1 // 3;", [
          new Token(TokenType.Const),
          new Token(TokenType.Identifier, new TokenValue("value")),
          new Token(TokenType.ColonTypeIndication),
          new Token(TokenType.Int),
          new Token(TokenType.Assignment),
          new Token(TokenType.NumberLiteral, new TokenValue("1")),
          new Token(TokenType.IntegerDivision),
          new Token(TokenType.NumberLiteral, new TokenValue("3")),
          new Token(TokenType.Semicolon),
          ]
      },
    };
  }

  private static string Normalize(string s) => string.Concat(s.Where(c => !char.IsWhiteSpace(c)));

  private static List<Token> Tokenize(string text)
  {
    List<Token> results = [];
    Lexer lexer = new(text);

    for (Token t = lexer.ParseToken(); t.Type != TokenType.EndOfFile; t = lexer.ParseToken())
    {
      results.Add(t);
    }

    return results;
  }
}
