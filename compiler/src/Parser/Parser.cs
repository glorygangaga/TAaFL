using Execution;

using Lexer;

namespace Parser;

/// <summary>
/// Выполняет синтаксический разбор.
/// Грамматика языка описана в файле `docs/specification/expressions-grammar.md`.
/// </summary>
public class Parser(Context context, IEnvironment environmentParser, string code)
{
  private readonly TokenStream tokens = new TokenStream(code);
  private readonly Context context = context;
  private readonly IEnvironment environment = environmentParser;

  public void ParseProgram()
  {
    context.PushScope(new Scope());
    do
    {
      Token t = tokens.Peek();

      switch (t.Type)
      {
        case TokenType.Const:
        case TokenType.Let:
          ParseValueDeclaration();
          break;
        case TokenType.Print:
          ParsePrintExpr();
          break;
        default:
          if (t.Type == TokenType.Identifier)
          {
            string name = t.Value!.ToString();
            tokens.Advance();
            if (tokens.Peek().Type == TokenType.Assignment)
            {
              ParseAssignableExpr(name);
            }
            else
            {
              decimal value = ParseExpr();
              Match(TokenType.Semicolon);
              environment.WriteNumber(value);
            }

            break;
          }
          else
          {
            decimal value = ParseExpr();
            Match(TokenType.Semicolon);
            environment.WriteNumber(value);

            break;
          }
      }
    }
    while (tokens.Peek().Type != TokenType.EndOfFile);
  }

  /// <summary>
  /// Парсинг переменных
  /// value_declaration = variable_declaration | constant_declaration;.
  /// </summary>
  private void ParseValueDeclaration()
  {
    switch (tokens.Peek().Type)
    {
      case TokenType.Const:
        ParseConstantDeclaration();
        break;
      case TokenType.Let:
        ParseVariableDeclaration();
        break;
    }
  }

  /// <summary>
  /// Парсинг let
  /// variable_declaration = "let", identifier, ":", type [ "=", expression ], ";";.
  /// </summary>
  private void ParseVariableDeclaration()
  {
    tokens.Advance();
    string name = tokens.Peek().Value!.ToString();
    tokens.Advance();
    Match(TokenType.ColonTypeIndication);

    // TokenType type = tokens.Peek().Type;
    tokens.Advance();

    decimal value = 0;

    if (tokens.Peek().Type == TokenType.Assignment)
    {
      tokens.Advance();
      value = ParseExpr();
    }

    Match(TokenType.Semicolon);

    context.DefineVariable(name, value);
  }

  /// <summary>
  /// Parsing AssignableExpr
  /// assignable_expr, "=", expression, ";" ;.
  /// </summary>
  private void ParseAssignableExpr(string name)
  {
    tokens.Advance();
    decimal value = ParseExpr();
    Match(TokenType.Semicolon);
    context.AssignVariable(name, value);
  }

  /// <summary>
  /// constant_declaration = "const", identifier, ":", type "=", expression, ";";.
  /// </summary>
  private void ParseConstantDeclaration()
  {
    tokens.Advance();
    Token t = tokens.Peek();

    if (t.Type != TokenType.Identifier || t.Value == null)
    {
      throw new UnexpectedLexemeException(TokenType.Identifier, t);
    }

    string name = tokens.Peek().Value!.ToString();
    tokens.Advance();
    Match(TokenType.ColonTypeIndication);

    // TokenType type = tokens.Peek().Type;
    tokens.Advance();
    Match(TokenType.Assignment);
    decimal value = ParseExpr();
    Match(TokenType.Semicolon);

    context.DefineConstant(name, value);
  }

  /// <summary>
  /// Парсинг нескольких выражений
  /// expression_list = expression, { ",", expression } ;.
  /// </summary>
  private List<decimal> ParseExpressionList()
  {
    List<decimal> values = new List<decimal> { ParseExpr() };
    while (tokens.Peek().Type == TokenType.Comma)
    {
      tokens.Advance();
      values.Add(ParseExpr());
    }

    return values;
  }

  /// <summary>
  /// Выполняет парсинг одного выражения.
  /// expression = or_expr.
  /// </summary>
  private decimal ParseExpr()
  {
    return ParseTernaryExpr();
  }

  /// <summary>
  /// Выполняет парсинг тернарного оператора
  /// ternary_expr = or_expr, [ "?", expression, ":", ternary_expr ] ;.
  /// </summary>
  private decimal ParseTernaryExpr()
  {
    decimal value = ParseOrExpr();
    if (tokens.Peek().Type == TokenType.QuestionMark)
    {
      tokens.Advance();
      decimal trueValue = ParseExpr();
      Match(TokenType.ColonTypeIndication);
      decimal falseValue = ParseTernaryExpr();
      return value != 0 ? trueValue : falseValue;
    }

    return value;
  }

  /// <summary>
  /// Выполняет парсинг выражения или.
  /// or_expr = and_expr, { "or", and_expr }.
  /// </summary>
  private decimal ParseOrExpr()
  {
    decimal value = ParseAndExpr();
    while (tokens.Peek().Type == TokenType.Or)
    {
      tokens.Advance();
      decimal right = ParseAndExpr();
      value = (value != 0 || right != 0) ? 1 : 0;
    }

    return value;
  }

  /// <summary>
  /// Выполняет парсинг выражения и.
  /// and_expr = equality_expr, { "and", equality_expr }.
  /// </summary>
  private decimal ParseAndExpr()
  {
    decimal value = ParseEqualityExpr();
    while (tokens.Peek().Type == TokenType.And)
    {
      tokens.Advance();
      decimal right = ParseEqualityExpr();
      value = (value != 0 && right != 0) ? 1 : 0;
    }

    return value;
  }

  /// <summary>
  /// Выполняет парсинг сравнения выражений.
  /// equality_expr = comparison_expr, { ("==" | "!="), comparison_expr } ;.
  /// </summary>
  private decimal ParseEqualityExpr()
  {
    decimal value = ParseComparisonExpr();
    while (true)
    {
      switch (tokens.Peek().Type)
      {
        case TokenType.Equal:
          tokens.Advance();
          value = (value == ParseComparisonExpr()) ? 1 : 0;
          break;
        case TokenType.NotEqual:
          tokens.Advance();
          value = (value != ParseComparisonExpr()) ? 1 : 0;
          break;
        default:
          return value;
      }
    }
  }

  /// <summary>
  /// Выполняет парсинг сравнения выражений.
  /// comparison_expr = arith_expr, { (".<" | ">" | "<=" | ">="), arith_expr }.
  /// </summary>
  private decimal ParseComparisonExpr()
  {
    decimal value = ParseArithExpr();
    while (true)
    {
      switch (tokens.Peek().Type)
      {
        case TokenType.LessThan:
          tokens.Advance();
          value = (value < ParseArithExpr()) ? 1 : 0;
          break;
        case TokenType.GreaterThan:
          tokens.Advance();
          value = (value > ParseArithExpr()) ? 1 : 0;
          break;
        case TokenType.LessThanOrEqual:
          tokens.Advance();
          value = (value <= ParseArithExpr()) ? 1 : 0;
          break;
        case TokenType.GreaterThanOrEqual:
          tokens.Advance();
          value = (value >= ParseArithExpr()) ? 1 : 0;
          break;
        default:
          return value;
      }
    }
  }

  /// <summary>
  /// Выполняет парсинг сложения/вычитания.
  /// arith_expr = term_expr, { ("+" | "-"), term_expr }.
  /// </summary>
  private decimal ParseArithExpr()
  {
    decimal value = ParseTermExpr();

    while (true)
    {
      switch (tokens.Peek().Type)
      {
        case TokenType.Plus:
          tokens.Advance();
          value += ParseTermExpr();
          break;
        case TokenType.Minus:
          tokens.Advance();
          value -= ParseTermExpr();
          break;
        default:
          return value;
      }
    }
  }

  /// <summary>
  /// Разбирает один операнд
  /// term_expr = prefix_expr, { ("\*" | "/" | "%" | "//"), prefix_expr } ;.
  /// </summary>
  private decimal ParseTermExpr()
  {
    decimal value = ParsePrefixExpr();
    while (true)
    {
      switch (tokens.Peek().Type)
      {
        case TokenType.Multiplication:
          tokens.Advance();
          value *= ParsePrefixExpr();
          break;
        case TokenType.Division:
          tokens.Advance();
          {
            decimal divisor = ParsePrefixExpr();
            if (divisor == 0)
            {
              throw new DivideByZeroException();
            }

            value /= divisor;
          }

          break;
        case TokenType.Remainder:
          tokens.Advance();
          {
            decimal divisor = ParsePrefixExpr();
            if (divisor == 0)
            {
              throw new DivideByZeroException();
            }

            value %= divisor;
          }

          break;
        case TokenType.IntegerDivision:
          tokens.Advance();
          {
            decimal divisor = ParsePrefixExpr();
            if (divisor == 0)
            {
              throw new DivideByZeroException();
            }

            value = Math.Floor(value / divisor);
          }

          break;
        default:
          return value;
      }
    }
  }

  /// <summary>
  /// Разбирает один префиксный операнд
  /// prefix_expr = { prefix_operator }, power_expr ;.
  /// </summary>
  private decimal ParsePrefixExpr()
  {
    switch (tokens.Peek().Type)
    {
      case TokenType.Increment:
        tokens.Advance();
        return ParsePrefixExpr() + 1;
      case TokenType.Decrement:
        tokens.Advance();
        return ParsePrefixExpr() - 1;
      case TokenType.Plus:
        tokens.Advance();
        return +ParsePrefixExpr();
      case TokenType.Minus:
        tokens.Advance();
        return -ParsePrefixExpr();
      case TokenType.Not:
        tokens.Advance();
        return ParsePrefixExpr() == 0 ? 1 : 0;
      default:
        return ParsePowerExpr();
    }
  }

  /// <summary>
  /// Разбирает одну операцию возведения в степень.
  /// power_expr = postfix_expr, { "\*\*", power_expr } ;.
  /// </summary>
  private decimal ParsePowerExpr()
  {
    decimal value = ParsePostfixExpr();
    while (tokens.Peek().Type == TokenType.Exponentiation)
    {
      tokens.Advance();
      decimal right = ParsePowerExpr();
      value = (decimal)Math.Pow((double)value, (double)right);
    }

    return value;
  }

  /// <summary>
  ///  Разбирает постфиксную операцию.
  /// postfix_expr = primary_expr, { postfix_operator } ;.
  /// </summary>
  private decimal ParsePostfixExpr()
  {
    decimal value = ParsePrimaryExpr();

    while (true)
    {
      switch (tokens.Peek().Type)
      {
        case TokenType.Increment:
          tokens.Advance();
          value++;
          continue;
        case TokenType.Decrement:
          tokens.Advance();
          value--;
          continue;
        case TokenType.OpenParenthesis:
        case TokenType.DotFieldAccess:
        case TokenType.OpenBlockComments:
          value = ParsePostfixOperator();
          continue;

        default: return value;
      }
    }
  }

  /// <summary>
  /// парсинг основного выражения
  /// primary_expr = identifier | literal | boolean | constant | array_literal | struct_literal | input_expr | print_expr | "(", expression, ")" ;.
  /// </summary>
  private decimal ParsePrimaryExpr()
  {
    Token t = tokens.Peek();
    switch (t.Type)
    {
      case TokenType.Identifier:
        string name = t.Value!.ToString();
        tokens.Advance();

        // if (tokens.Peek().Type == TokenType.OpenCurlyBrace)
        // {
        //   return ParseStructLiteral();
        // }
        return context.GetValue(name);

      case TokenType.Min:
      case TokenType.Max:
      case TokenType.Ceil:
      case TokenType.Floor:
      case TokenType.Round:
      case TokenType.Pow:
      case TokenType.Abs:
        return ParseFunctionCall();

      case TokenType.NumberLiteral:
      case TokenType.StringLiteral:
        return ParseLiteral();
      case TokenType.True:
      case TokenType.False:
        return ParseBool();
      case TokenType.Pi:
      case TokenType.Euler:
        return ParseConstant();

      // case TokenType.OpenSquareBracket:
      // return ParseArrayLiteral();
      // break;
      case TokenType.OpenParenthesis:
        tokens.Advance();
        decimal value = ParseExpr();
        Match(TokenType.CloseParenthesis);
        return value;

      case TokenType.Input:
        tokens.Advance();
        return ParseInput();
      default:
        throw new UnexpectedLexemeException(t.Type, t);
    }
  }

  /// <summary>
  /// Парсинг инпута
  /// input_expr = "input", "(", [ expression ], ")" ;.
  /// </summary>
  private decimal ParseInput()
  {
    Match(TokenType.OpenParenthesis);
    Match(TokenType.CloseParenthesis);

    return environment.ReadNumber();
  }

  /// <summary>
  /// Парсинг вывода
  /// print_expr = "print", "(", [expression_list], ")" ;.
  /// </summary>
  private void ParsePrintExpr()
  {
    tokens.Advance();
    Match(TokenType.OpenParenthesis);
    List<decimal> values = ParseExpressionList();
    Match(TokenType.CloseParenthesis);
    Match(TokenType.Semicolon);
    for (int i = 0; i < values.Count; i++)
    {
      environment.WriteNumber(values[i]);
    }
  }

  /// <summary>
  /// Парсинг структуры
  /// struct_literal = identifier, "{", [ field_initializer_list ], "}" ;.
  /// </summary>
  private List<decimal> ParseStructLiteral()
  {
    Match(TokenType.OpenCurlyBrace);
    List<decimal> values = ParseStructInitList();
    Match(TokenType.CloseCurlyBracket);
    return values;
  }

  /// <summary>
  /// Парсинг иницилизации структуры
  /// field_initializer_list = field_initializer, { ",", field_initializer }.
  /// </summary>
  private List<decimal> ParseStructInitList()
  {
    List<decimal> list = new List<decimal> { ParseStructInit() };
    while (tokens.Peek().Type == TokenType.Comma)
    {
      tokens.Advance();
      list.Add(ParseStructInit());
    }

    return list;
  }

  /// <summary>
  /// Парсинг поля структуры
  /// field_initializer = identifier, ":", expression.
  /// </summary>
  private decimal ParseStructInit()
  {
    Match(TokenType.Identifier);
    Match(TokenType.ColonTypeIndication);
    return ParseExpr();
  }

  /// <summary>
  /// Парсинг массива
  /// array_literal = "[", [ expression_list ], "]".
  /// </summary>
  private List<decimal> ParseArrayLiteral()
  {
    Match(TokenType.OpenSquareBracket);
    List<decimal> values = ParseExpressionList();
    Match(TokenType.CloseSquareBracket);
    return values;
  }

  /// <summary>
  ///  Парсинг констант
  ///  constant = "Pi" | "Euler".
  /// </summary>
  private decimal ParseConstant()
  {
    Token t = tokens.Peek();
    tokens.Advance();

    return t.Type switch
    {
      TokenType.Euler => 2.71828182846M,
      TokenType.Pi => 3.14159265358M,
      _ => throw new UnexpectedLexemeException(t.Type, t),
    };
  }

  /// <summary>
  /// Парсинг литерала
  /// literal = number | string.
  /// </summary>
  private decimal ParseLiteral()
  {
    Token t = tokens.Peek();
    switch (t.Type)
    {
      case TokenType.NumberLiteral:
        decimal value = t.Value!.ToDecimal();
        tokens.Advance();
        return value;

      // case TokenType.StringLiteral:
      //   break;
      default:
        throw new UnexpectedLexemeException(t.Type, t);
    }
  }

  /// <summary>
  /// Парсинг логического значения
  /// boolean = "true" | "false" ;.
  /// </summary>
  private decimal ParseBool()
  {
    Token t = tokens.Peek();
    tokens.Advance();
    return t.Type switch
    {
      TokenType.True => 1,
      TokenType.False => 0,
      _ => throw new UnexpectedLexemeException(t.Type, t),
    };
  }

  /// <summary>
  /// Парсинг постфиксных операторов.
  /// postfix_operator = function_call | member_access | index_access | "++"  | "--".
  /// </summary>
  private decimal ParsePostfixOperator()
  {
    Token t = tokens.Peek();

    return t.Type switch
    {
      TokenType.OpenParenthesis => ParseFunctionCall(),
      TokenType.DotFieldAccess => ParseMemerAccess(),
      TokenType.OpenSquareBracket => ParseIndexAccess(),
      _ => throw new UnexpectedLexemeException(t.Type, t),
    };
  }

  /// <summary>
  /// Парсинг вызова функции
  /// function_call = "(", [ argument_list ], ")" ;.
  /// </summary>
  private decimal ParseFunctionCall()
  {
    Token nameToken = tokens.Peek();
    tokens.Advance();
    Match(TokenType.OpenParenthesis);
    List<decimal> args = ParseExpressionList();
    Match(TokenType.CloseParenthesis);

    return BuiltinFunctions.Instance.Invoke(nameToken.Type, args);
  }

  /// <summary>
  /// Парсинг вызова поля в структуре
  /// member_access = ".", identifier ;.
  /// </summary>
  private decimal ParseMemerAccess()
  {
    Match(TokenType.DotFieldAccess);
    Match(TokenType.Identifier);
    return 1;
  }

  /// <summary>
  /// парсинг индекса в массиве
  /// index_access = "[", expression, "]" ;.
  /// </summary>
  private decimal ParseIndexAccess()
  {
    Match(TokenType.OpenSquareBracket);
    decimal value = ParseExpr();
    Match(TokenType.CloseSquareBracket);
    return value;
  }

  private void Match(TokenType expected)
  {
    if (tokens.Peek().Type != expected)
    {
      throw new UnexpectedLexemeException(expected, tokens.Peek());
    }

    tokens.Advance();
  }
}
