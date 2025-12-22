namespace Lexer;

public class Lexer(string text)
{
  private static readonly Dictionary<string, TokenType> Keywords = new()
  {
    { "const", TokenType.Const },
    { "let", TokenType.Let },
    { "str", TokenType.Str },
    { "int", TokenType.Int },
    { "bool", TokenType.Bool },
    { "float", TokenType.Float },
    { "void", TokenType.Void },
    { "true", TokenType.True },
    { "false", TokenType.False },
    { "not", TokenType.Not },
    { "and", TokenType.And },
    { "or", TokenType.Or },
    { "if", TokenType.If },
    { "elif", TokenType.Elif },
    { "else", TokenType.Else },
    { "for", TokenType.For },
    { "while", TokenType.While },
    { "func", TokenType.Func },
    { "return", TokenType.Return },
    { "import", TokenType.Import },
    { "input", TokenType.Input },
    { "print", TokenType.Print },
    { "Pi", TokenType.Pi },
    { "Euler", TokenType.Euler },
    { "abs", TokenType.Abs },
    { "min", TokenType.Min },
    { "max", TokenType.Max },
    { "pow", TokenType.Pow },
    { "round", TokenType.Round },
    { "ceil", TokenType.Ceil },
    { "floor", TokenType.Floor },
    { "main", TokenType.Main },
    { "switch", TokenType.Switch },
    { "case", TokenType.Case },
    { "break", TokenType.Break },
    { "continue", TokenType.Continue },
    { "default", TokenType.Default },
  };

  private readonly TextScanner scanner = new TextScanner(text);

  public Token ParseToken()
  {
    SkipWhiteSpaces();

    if (scanner.IsEnd())
    {
      return new Token(TokenType.EndOfFile);
    }

    char ch = scanner.Peek();

    if (char.IsLetter(ch) || ch == '_')
    {
      return ParseIdentifierOrKeyword();
    }
    else if (char.IsAsciiDigit(ch))
    {
      return ParseNumberLiteral();
    }
    else if (ch == '"')
    {
      return ParseStringLiteral();
    }
    else
    {
      return ParseRemainTokens();
    }
  }

  private Token ParseIdentifierOrKeyword()
  {
    string identifier = "";
    for (char ch = scanner.Peek(); ch == '_' || char.IsLetter(ch) || char.IsAsciiDigit(ch); ch = scanner.Peek())
    {
      identifier += ch;
      scanner.Advance();
    }

    if (Keywords.TryGetValue(identifier, out TokenType keyword))
    {
      return new Token(keyword);
    }
    else
    {
      return new Token(TokenType.Identifier, new TokenValue(identifier));
    }
  }

  private Token ParseNumberLiteral()
  {
    string value = "";

    while (char.IsAsciiDigit(scanner.Peek()))
    {
      value += scanner.Peek();
      scanner.Advance();
    }

    if (scanner.Peek() == '.')
    {
      value += scanner.Peek();
      scanner.Advance();

      while (char.IsAsciiDigit(scanner.Peek()))
      {
        value += scanner.Peek();
        scanner.Advance();
      }
    }

    decimal number;
    decimal.TryParse(value, out number);

    return new Token(TokenType.NumberLiteral, new TokenValue(number));
  }

  private Token ParseStringLiteral()
  {
    scanner.Advance();
    string content = "";
    while (scanner.Peek() != '"')
    {
      if (scanner.Peek() == '\n' || scanner.Peek() == '\r' || scanner.IsEnd())
      {
        scanner.Advance();
        return new Token(TokenType.Error, new TokenValue(content));
      }

      if (TryParseStringSpecialCharacters(out char specialChar))
      {
        content += specialChar;
      }
      else
      {
        content += scanner.Peek();
        scanner.Advance();
      }
    }

    scanner.Advance();
    return new Token(TokenType.StringLiteral, new TokenValue(content));
  }

  private bool TryParseStringSpecialCharacters(out char specialChar)
  {
    if (scanner.Peek() == '\\')
    {
      scanner.Advance();

      switch (scanner.Peek())
      {
        case 'n':
          specialChar = '\n';
          break;
        case 't':
          specialChar = '\t';
          break;
        case '\\':
          specialChar = '\\';
          break;
        case '"':
          specialChar = '\"';
          break;
        default:
          specialChar = '\0';
          break;
      }

      scanner.Advance();
      return true;
    }

    specialChar = '\0';
    return false;
  }

  private Token ParseRemainTokens()
  {
    char ch = scanner.Peek();
    scanner.Advance();

    switch (ch)
    {
      case ':':
        return new Token(TokenType.ColonTypeIndication);
      case ';':
        return new Token(TokenType.Semicolon);
      case ',':
        return new Token(TokenType.Comma);
      case '(':
        return new Token(TokenType.OpenParenthesis);
      case ')':
        return new Token(TokenType.CloseParenthesis);
      case '{':
        return new Token(TokenType.OpenCurlyBrace);
      case '}':
        return new Token(TokenType.CloseCurlyBracket);
      case '?':
        return new Token(TokenType.QuestionMark);
      case '>':
        if (scanner.Peek() == '=')
        {
          scanner.Advance();
          return new Token(TokenType.GreaterThanOrEqual);
        }

        return new Token(TokenType.GreaterThan);
      case '<':
        if (scanner.Peek() == '=')
        {
          scanner.Advance();
          return new Token(TokenType.LessThanOrEqual);
        }

        return new Token(TokenType.LessThan);

      case '=':
        if (scanner.Peek() == '=')
        {
          scanner.Advance();
          return new Token(TokenType.Equal);
        }

        return new Token(TokenType.Assignment);
      case '!':
        if (scanner.Peek() == '=')
        {
          scanner.Advance();
          return new Token(TokenType.NotEqual);
        }

        return new Token(TokenType.Error, new TokenValue(char.ToString(scanner.Peek())));
      case '+':
        if (scanner.Peek() == '+')
        {
          scanner.Advance();
          return new Token(TokenType.Increment);
        }

        return new Token(TokenType.Plus);
      case '-':
        if (scanner.Peek() == '-')
        {
          scanner.Advance();
          return new Token(TokenType.Decrement);
        }

        return new Token(TokenType.Minus);
      case '*':
        if (scanner.Peek() == '*')
        {
          scanner.Advance();
          return new Token(TokenType.Exponentiation);
        }
        else if (scanner.Peek() == '/')
        {
          scanner.Advance();
          return new Token(TokenType.CloseBlockComments);
        }

        return new Token(TokenType.Multiplication);
      case '/':
        if (scanner.Peek() == '/')
        {
          if (scanner.Peek(1) == '/')
          {
            scanner.Advance();
            SkipSingleLineComment();
            return new Token(TokenType.SingleLineComment);
          }

          scanner.Advance();
          return new Token(TokenType.IntegerDivision);
        }
        else if (scanner.Peek() == '*')
        {
          SkipBlockComments();
          return new Token(TokenType.OpenBlockComments);
        }

        return new Token(TokenType.Division);
      case '%':
        return new Token(TokenType.Remainder);
      default:
        return new Token(TokenType.Error, new TokenValue(ch.ToString()));
    }
  }

  private void SkipWhiteSpaces()
  {
    while (char.IsWhiteSpace(scanner.Peek()))
    {
      scanner.Advance();
    }
  }

  private void SkipSingleLineComment()
  {
    scanner.Advance();
    while (scanner.Peek() != '\n' && !scanner.IsEnd())
    {
      scanner.Advance();
    }
  }

  private void SkipBlockComments()
  {
    scanner.Advance();
    while (scanner.Peek() != '*' && scanner.Peek(1) != '/' && !scanner.IsEnd())
    {
      scanner.Advance();
    }
  }
}
