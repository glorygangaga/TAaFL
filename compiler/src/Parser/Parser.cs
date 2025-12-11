using Ast;
using Ast.Declarations;
using Ast.Expressions;

using Execution;

using Lexer;

namespace Parser;

/// <summary>
/// Выполняет синтаксический разбор.
/// Грамматика языка описана в файле `docs/specification/expressions-grammar.md`.
/// </summary>
public class Parser
{
  private readonly TokenStream tokens;
  private readonly AstEvaluator evaluator;
  private readonly IEnvironment environment;
  private readonly Context context;

  public Parser(string source)
  {
    tokens = new TokenStream(source);
    environment = new FakeEnvironment();
    context = new Context(environment);

    evaluator = new AstEvaluator(context);
  }

  public Parser(Context context, IEnvironment environmentParser, string code)
  {
    environment = environmentParser;
    tokens = new TokenStream(code);
    this.context = context;
    evaluator = new AstEvaluator(context);
  }

  public void ParseProgram()
  {
    while (tokens.Peek().Type != TokenType.EndOfFile)
    {
      AstNode node = ParseTopLevelDeclaration();
      evaluator.Evaluate(node);

      Match(TokenType.Semicolon);

      // if (tokens.Peek().Type == TokenType.Semicolon)
      // {
      //   tokens.Advance();
      // }
    }
  }

  /// <summary>
  /// top_level_declaration = value_declaration | function_declaration | struct_declaration;.
  /// </summary>
  private AstNode ParseTopLevelDeclaration()
  {
    switch (tokens.Peek().Type)
    {
      case TokenType.Const:
      case TokenType.Let:
        return ParseValueDeclaration();
      case TokenType.Func:
        return ParseFunctionDeclaration();
      default:
        return ParseExpr();
    }
  }

  /// <summary>
  /// "func", identifier, ":", type, "(", [ parameter_list ], ")", block ;.
  /// </summary>
  private AstNode ParseFunctionDeclaration()
  {
    Match(TokenType.Func);
    string name = tokens.Peek().Value!.ToString();
    Match(TokenType.ColonTypeIndication);
    Match(TokenType.Int);
    Match(TokenType.OpenParenthesis);
    List<string> parameters = ParseParameterList();
    Expression value = ParseExpr();
    return new FunctionDeclaration(name, parameters, value);
  }

  /// <summary>
  /// parameter_list = parameter, { ",", parameter } ;.
  /// </summary>
  private List<string> ParseParameterList()
  {
    List<string> parameters = [];

    while (tokens.Peek().Type != TokenType.CloseParenthesis)
    {
      string parameter = Match(TokenType.Identifier).Value!.ToString();
      Match(TokenType.ColonTypeIndication);
      Match(TokenType.Int);
      parameters.Add(parameter);

      if (tokens.Peek().Type != TokenType.Comma)
      {
        break;
      }

      tokens.Advance();
    }

    Match(TokenType.CloseParenthesis);

    return parameters;
  }

  /// <summary>
  /// Парсинг переменных
  /// value_declaration = variable_declaration | constant_declaration;.
  /// </summary>
  private AstNode ParseValueDeclaration()
  {
    switch (tokens.Peek().Type)
    {
      case TokenType.Const:
        return ParseConstantDeclaration();
      case TokenType.Let:
        return ParseVariableDeclaration();
      default:
        throw new Exception("");
    }
  }

  /// <summary>
  /// Парсинг let
  /// variable_declaration = "let", identifier, ":", type [ "=", expression ], ";";.
  /// </summary>
  private AstNode ParseVariableDeclaration()
  {
    tokens.Advance();
    string name = tokens.Peek().Value!.ToString();
    tokens.Advance();
    Match(TokenType.ColonTypeIndication);
    Match(TokenType.Int);

    Expression? value = null;

    if (tokens.Peek().Type == TokenType.Assignment)
    {
      tokens.Advance();
      value = ParseExpr();
    }

    return new VariableDeclaration(name, value);
  }

  /// <summary>
  /// Parsing AssignableExpr
  /// assignable_expr, "=", expression, ";" ;.
  /// </summary>
  private Expression ParseAssignableExpr(string name)
  {
    tokens.Advance();
    Expression value = ParseExpr();
    return new AssignmentExpression(name, value);
  }

  /// <summary>
  /// constant_declaration = "const", identifier, ":", type "=", expression, ";";.
  /// </summary>
  private AstNode ParseConstantDeclaration()
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
    Match(TokenType.Int);
    Match(TokenType.Assignment);
    Expression value = ParseExpr();

    return new ConstantDeclaration(name, value);
  }

  /// <summary>
  /// Парсинг нескольких выражений
  /// expression_list = expression, { ",", expression } ;.
  /// </summary>
  private List<Expression> ParseExpressionList()
  {
    List<Expression> values = new List<Expression> { ParseExpr() };
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
  private Expression ParseExpr()
  {
    return ParseTernaryExpr();
  }

  /// <summary>
  /// Выполняет парсинг тернарного оператора
  /// ternary_expr = or_expr, [ "?", expression, ":", ternary_expr ] ;.
  /// </summary>
  private Expression ParseTernaryExpr()
  {
    Expression value = ParseOrExpr();
    if (tokens.Peek().Type == TokenType.QuestionMark)
    {
      tokens.Advance();
      Expression trueValue = ParseExpr();
      Match(TokenType.ColonTypeIndication);
      Expression falseValue = ParseTernaryExpr();
      return new IfElseExpression(value, trueValue, falseValue);
    }

    return value;
  }

  /// <summary>
  /// Выполняет парсинг выражения или.
  /// or_expr = and_expr, { "or", and_expr }.
  /// </summary>
  private Expression ParseOrExpr()
  {
    Expression value = ParseAndExpr();
    while (tokens.Peek().Type == TokenType.Or)
    {
      tokens.Advance();
      Expression right = ParseAndExpr();
      value = new BinaryOperationExpression(value, BinaryOperation.Or, right);
    }

    return value;
  }

  /// <summary>
  /// Выполняет парсинг выражения и.
  /// and_expr = equality_expr, { "and", equality_expr }.
  /// </summary>
  private Expression ParseAndExpr()
  {
    Expression value = ParseEqualityExpr();
    while (tokens.Peek().Type == TokenType.And)
    {
      tokens.Advance();
      Expression right = ParseEqualityExpr();
      value = new BinaryOperationExpression(value, BinaryOperation.And, right);
    }

    return value;
  }

  /// <summary>
  /// Выполняет парсинг сравнения выражений.
  /// equality_expr = comparison_expr, { ("==" | "!="), comparison_expr } ;.
  /// </summary>
  private Expression ParseEqualityExpr()
  {
    Expression value = ParseComparisonExpr();
    while (true)
    {
      switch (tokens.Peek().Type)
      {
        case TokenType.Equal:
          tokens.Advance();
          value = new BinaryOperationExpression(value, BinaryOperation.Equal, ParseComparisonExpr());
          break;
        case TokenType.NotEqual:
          tokens.Advance();
          value = new BinaryOperationExpression(value, BinaryOperation.NotEqual, ParseComparisonExpr());
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
  private Expression ParseComparisonExpr()
  {
    Expression value = ParseArithExpr();
    while (true)
    {
      switch (tokens.Peek().Type)
      {
        case TokenType.LessThan:
          tokens.Advance();
          value = new BinaryOperationExpression(value, BinaryOperation.LessThan, ParseArithExpr());
          break;
        case TokenType.GreaterThan:
          tokens.Advance();
          value = new BinaryOperationExpression(value, BinaryOperation.GreaterThan, ParseArithExpr());
          break;
        case TokenType.LessThanOrEqual:
          tokens.Advance();
          value = new BinaryOperationExpression(value, BinaryOperation.LessThanOrEqual, ParseArithExpr());
          break;
        case TokenType.GreaterThanOrEqual:
          tokens.Advance();
          value = new BinaryOperationExpression(value, BinaryOperation.GreaterThanOrEqual, ParseArithExpr());
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
  private Expression ParseArithExpr()
  {
    Expression value = ParseTermExpr();

    while (true)
    {
      switch (tokens.Peek().Type)
      {
        case TokenType.Plus:
          tokens.Advance();
          value = new BinaryOperationExpression(value, BinaryOperation.Plus, ParseTermExpr());
          break;
        case TokenType.Minus:
          tokens.Advance();
          value = new BinaryOperationExpression(value, BinaryOperation.Minus, ParseTermExpr());
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
  private Expression ParseTermExpr()
  {
    Expression value = ParsePrefixExpr();
    while (true)
    {
      switch (tokens.Peek().Type)
      {
        case TokenType.Multiplication:
          tokens.Advance();
          value = new BinaryOperationExpression(value, BinaryOperation.Multiplication, ParsePrefixExpr());
          break;
        case TokenType.Division:
          tokens.Advance();
          value = new BinaryOperationExpression(value, BinaryOperation.Division, ParsePrefixExpr());
          break;
        case TokenType.Remainder:
          tokens.Advance();
          value = new BinaryOperationExpression(value, BinaryOperation.Remainder, ParsePrefixExpr());
          break;
        case TokenType.IntegerDivision:
          tokens.Advance();
          value = new BinaryOperationExpression(value, BinaryOperation.IntegerDivision, ParsePrefixExpr());
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
  private Expression ParsePrefixExpr()
  {
    switch (tokens.Peek().Type)
    {
      case TokenType.Increment:
        tokens.Advance();
        return new UnaryOperationExpression(UnaryOperation.Increment, ParsePrefixExpr());
      case TokenType.Decrement:
        tokens.Advance();
        return new UnaryOperationExpression(UnaryOperation.Decrement, ParsePrefixExpr());
      case TokenType.Plus:
        tokens.Advance();
        return new UnaryOperationExpression(UnaryOperation.Plus, ParsePrefixExpr());
      case TokenType.Minus:
        tokens.Advance();
        return new UnaryOperationExpression(UnaryOperation.Minus, ParsePrefixExpr());
      case TokenType.Not:
        tokens.Advance();
        return new UnaryOperationExpression(UnaryOperation.Not, ParsePrefixExpr());
      default:
        return ParsePowerExpr();
    }
  }

  /// <summary>
  /// Разбирает одну операцию возведения в степень.
  /// power_expr = postfix_expr, { "\*\*", power_expr } ;.
  /// </summary>
  private Expression ParsePowerExpr()
  {
    Expression value = ParsePostfixExpr();
    while (tokens.Peek().Type == TokenType.Exponentiation)
    {
      tokens.Advance();
      Expression right = ParsePowerExpr();
      value = new BinaryOperationExpression(value, BinaryOperation.Exponentiation, right);
    }

    return value;
  }

  /// <summary>
  ///  Разбирает постфиксную операцию.
  /// postfix_expr = primary_expr, { postfix_operator } ;.
  /// </summary>
  private Expression ParsePostfixExpr()
  {
    Expression value = ParsePrimaryExpr();

    while (true)
    {
      switch (tokens.Peek().Type)
      {
        case TokenType.Increment:
          tokens.Advance();
          value = new UnaryOperationExpression(UnaryOperation.Increment, value);

          continue;
        case TokenType.Decrement:
          tokens.Advance();
          value = new UnaryOperationExpression(UnaryOperation.Decrement, value);
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
  private Expression ParsePrimaryExpr()
  {
    Token t = tokens.Peek();
    switch (t.Type)
    {
      case TokenType.Identifier:
        string name = t.Value!.ToString();
        tokens.Advance();

        if (tokens.Peek().Type == TokenType.Assignment)
        {
          return ParseAssignableExpr(name);
        }
        else if (tokens.Peek().Type == TokenType.OpenParenthesis)
        {
          return ParseFunctionCall(name);
        }
        else
        {
          return new VariableExpression(name);
        }

      case TokenType.Min:
      case TokenType.Max:
      case TokenType.Ceil:
      case TokenType.Floor:
      case TokenType.Round:
      case TokenType.Pow:
      case TokenType.Abs:
        return ParseFunctionCall(BuildInFuncToString(tokens.Peek()));
      case TokenType.NumberLiteral:
        return ParseLiteral();
      case TokenType.True:
      case TokenType.False:
        return ParseBool();
      case TokenType.Pi:
      case TokenType.Euler:
        return ParseConstant();
      case TokenType.OpenParenthesis:
        tokens.Advance();
        Expression value = ParseExpr();
        Match(TokenType.CloseParenthesis);
        return value;
      case TokenType.Input:
        return ParseInput();
      case TokenType.Print:
        return ParsePrintExpr();
      default:
        throw new UnexpectedLexemeException(t.Type, t);
    }
  }

  private string BuildInFuncToString(Token token)
  {
    return token.Type switch
    {
      TokenType.Min => "min",
      TokenType.Max => "max",
      TokenType.Ceil => "ceil",
      TokenType.Floor => "floor",
      TokenType.Round => "round",
      TokenType.Pow => "pow",
      TokenType.Abs => "abs",
      _ => throw new UnexpectedLexemeException(token.Type, token),
    };
  }

  /// <summary>
  /// Парсинг инпута
  /// input_expr = "input", "(", [ expression ], ")" ;.
  /// </summary>
  private Expression ParseInput()
  {
    tokens.Advance();
    Match(TokenType.OpenParenthesis);
    Match(TokenType.CloseParenthesis);

    return new LiteralExpression(environment.ReadNumber());
  }

  /// <summary>
  /// Парсинг вывода
  /// print_expr = "print", "(", [expression_list], ")" ;.
  /// </summary>
  private Expression ParsePrintExpr()
  {
    tokens.Advance();
    Match(TokenType.OpenParenthesis);

    Expression values = ParseExpr();
    Match(TokenType.CloseParenthesis);
    return new PrintExpression(values);
  }

  /// <summary>
  /// Парсинг иницилизации структуры
  /// field_initializer_list = field_initializer, { ",", field_initializer }.
  /// </summary>
  private List<Expression> ParseStructInitList()
  {
    List<Expression> list = new List<Expression> { ParseStructInit() };
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
  private Expression ParseStructInit()
  {
    Match(TokenType.Identifier);
    Match(TokenType.ColonTypeIndication);
    return ParseExpr();
  }

  /// <summary>
  ///  Парсинг констант
  ///  constant = "Pi" | "Euler".
  /// </summary>
  private Expression ParseConstant()
  {
    Token t = tokens.Peek();
    tokens.Advance();

    return t.Type switch
    {
      TokenType.Euler => new LiteralExpression(2.71828182846M),
      TokenType.Pi => new LiteralExpression(3.14159265358M),
      _ => throw new UnexpectedLexemeException(t.Type, t),
    };
  }

  /// <summary>
  /// Парсинг литерала
  /// literal = number | string.
  /// </summary>
  private Expression ParseLiteral()
  {
    Token t = tokens.Peek();
    switch (t.Type)
    {
      case TokenType.NumberLiteral:
        tokens.Advance();
        return new LiteralExpression(t.Value!.ToDecimal());
      default:
        throw new UnexpectedLexemeException(t.Type, t);
    }
  }

  /// <summary>
  /// Парсинг логического значения
  /// boolean = "true" | "false" ;.
  /// </summary>
  private Expression ParseBool()
  {
    Token t = tokens.Peek();
    tokens.Advance();

    return t.Type switch
    {
      TokenType.True => new LiteralExpression(1.0m),
      TokenType.False => new LiteralExpression(0.0m),
      _ => throw new UnexpectedLexemeException(t.Type, t),
    };
  }

  /// <summary>
  /// Парсинг постфиксных операторов.
  /// postfix_operator = function_call | member_access | index_access | "++"  | "--".
  /// </summary>
  private Expression ParsePostfixOperator()
  {
    Token t = tokens.Peek();

    return t.Type switch
    {
      TokenType.OpenSquareBracket => ParseIndexAccess(),
      _ => throw new UnexpectedLexemeException(t.Type, t),
    };
  }

  /// <summary>
  /// Парсинг вызова функции
  /// function_call = "(", [ argument_list ], ")" ;.
  /// </summary>
  private Expression ParseFunctionCall(string name)
  {
    tokens.Advance();
    Match(TokenType.OpenParenthesis);
    List<Expression> args = ParseExpressionList();
    Match(TokenType.CloseParenthesis);

    return new FunctionCallExpression(name, args);
  }

  /// <summary>
  /// парсинг индекса в массиве
  /// index_access = "[", expression, "]" ;.
  /// </summary>
  private Expression ParseIndexAccess()
  {
    Match(TokenType.OpenSquareBracket);
    Expression value = ParseExpr();
    Match(TokenType.CloseSquareBracket);
    return value;
  }

  private Token Match(TokenType expected)
  {
    Token t = tokens.Peek() ?? new Token(TokenType.EndOfFile);

    if (t.Type != expected || t.Type == TokenType.EndOfFile)
    {
      throw new UnexpectedLexemeException(expected, t);
    }

    tokens.Advance();
    return t;
  }
}
