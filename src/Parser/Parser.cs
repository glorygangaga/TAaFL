using System.Globalization;

using Ast;
using Ast.Declarations;
using Ast.Expressions;

using Execution;

using Lexer;

using Runtime;

using ValueType = Runtime.ValueType;

namespace Parser;

/// <summary>
/// Выполняет синтаксический разбор.
/// Грамматика языка описана в файле `docs/specification/expressions-grammar.md`.
/// </summary>
public class Parser
{
  private readonly TokenStream tokens;
  private readonly IEnvironment environment;

  public Parser(string source)
  {
    tokens = new TokenStream(source);
    environment = new FakeEnvironment();
  }

  public Parser(IEnvironment environmentParser, string code)
  {
    environment = environmentParser;
    tokens = new TokenStream(code);
  }

  /// <summary>
  /// program = { top_level_declaration }, main_function ;.
  /// </summary>
  public Expression ParseProgram()
  {
    List<Expression> expressions = new();

    while (IsTopLevelDeclaration() && tokens.Peek().Type != TokenType.EndOfFile)
    {
      expressions.Add((Expression)ParseTopLevelDeclaration());
    }

    if (!IsTopLevelDeclaration())
    {
      expressions.Add((Expression)ParseMainFunction());
    }

    return new SequenceExpression(expressions);
  }

  private bool IsTopLevelDeclaration()
  {
    return !(tokens.Peek().Type == TokenType.Func && tokens.Peek(1).Type == TokenType.Main);
  }

  /// <summary>
  /// top_level_declaration = value_declaration | function_declaration;.
  /// </summary>
  private AstNode ParseTopLevelDeclaration()
  {
    switch (tokens.Peek().Type)
    {
      case TokenType.Const:
      case TokenType.Let:
        Declaration decl = ParseValueDeclaration();
        Match(TokenType.Semicolon);
        return decl;
      case TokenType.Func:
        return ParseFunctionDeclaration();
      default:
        Expression expr = ParseExpr();
        Match(TokenType.Semicolon);
        return expr;
    }
  }

  /// <summary>
  /// main_function = "func", "main", ":", "void", "(", ")", block ;.
  /// </summary>
  private AstNode ParseMainFunction()
  {
    Match(TokenType.Func);
    Match(TokenType.Main);
    Match(TokenType.ColonTypeIndication);
    Match(TokenType.Void);
    Match(TokenType.OpenParenthesis);
    Match(TokenType.CloseParenthesis);
    return ParseBlock();
  }

  /// <summary>
  /// block = "{", { statement }, "}" ;.
  /// </summary>
  private Expression ParseBlock()
  {
    Match(TokenType.OpenCurlyBrace);

    List<Expression> statements = [];

    while (tokens.Peek().Type != TokenType.CloseCurlyBracket)
    {
      Expression state = ParseStatement();
      statements.Add(state);
    }

    Match(TokenType.CloseCurlyBracket);

    return new SequenceExpression(statements);
  }

  /// <summary>
  /// statement = expression_statement | assignment_statement | value_declaration | block | input_statement | print_statement
  /// | if_statement | while_statement | for_statement | switch_statement | break_statement |
  /// continue_statement | return_statement | empty_statement ;.
  /// </summary>
  private Expression ParseStatement()
  {
    switch (tokens.Peek().Type)
    {
      case TokenType.OpenCurlyBrace:
        return ParseBlock();
      case TokenType.Print:
        return ParsePrintExpr();
      case TokenType.Input:
        return ParseInput();
      case TokenType.If:
        return ParseIfExpression();
      case TokenType.For:
        return ParseForExpr();
      case TokenType.While:
        return ParseWhileExpr();
      case TokenType.Return:
        return ParseReturnStatement();
      case TokenType.Break:
        return ParseBreakExpr();
      case TokenType.Continue:
        return ParseContinueExpr();
      case TokenType.Switch:
        return ParseSwitchExpr();
      case TokenType.Identifier:
        string name = tokens.Peek().Value!.ToString();
        tokens.Advance();

        Expression expr;
        if (tokens.Peek().Type == TokenType.Assignment)
        {
          expr = ParseAssignableExpr(name);
        }
        else if (tokens.Peek().Type == TokenType.OpenParenthesis)
        {
          expr = ParseFunctionCall(name);
        }
        else
        {
          throw new Exception();
        }

        Match(TokenType.Semicolon);

        return expr;
      case TokenType.Let:
      case TokenType.Const:
        Declaration decl = ParseValueDeclaration();
        Match(TokenType.Semicolon);
        return new DeclarationExpression(decl);
      case TokenType.Semicolon:
        tokens.Advance();
        return new EmptyExpression();
      case TokenType.SingleLineComment:
        tokens.Advance();
        return new EmptyExpression();
      case TokenType.OpenBlockComments:
        Match(TokenType.CloseBlockComments);
        tokens.Advance();
        return new EmptyExpression();
      default:
        Expression expression = ParseExpr();
        Match(TokenType.Semicolon);
        return expression;
    }
  }

  /// <summary>
  /// switch_statement = "switch", "(", expression, ")", "{", { case_clause }, [ default_clause ],"}" ;
  /// case_clause = "case", expression, ":", { statement } ;
  /// default_clause = "default", ":", { statement } ;.
  /// </summary>
  private Expression ParseSwitchExpr()
  {
    Match(TokenType.Switch);
    Match(TokenType.OpenParenthesis);
    Expression expr = ParseExpr();
    Match(TokenType.CloseParenthesis);

    Match(TokenType.OpenCurlyBrace);

    List<SwitchCase> cases = [];
    while (tokens.Peek().Type == TokenType.Case)
    {
      tokens.Advance();
      Expression value = ParseExpr();
      Match(TokenType.ColonTypeIndication);
      Expression body = ParseStatement();
      cases.Add(new SwitchCase(value, body));
    }

    Expression? defaultCase = null;
    if (tokens.Peek().Type == TokenType.Default)
    {
      tokens.Advance();
      Match(TokenType.ColonTypeIndication);
      defaultCase = ParseStatement();
    }

    Match(TokenType.CloseCurlyBracket);

    return new SwitchExpression(expr, cases, defaultCase);
  }

  /// <summary>
  /// continue_statement = "continue", ";" ;.
  /// </summary>
  private Expression ParseContinueExpr()
  {
    Match(TokenType.Continue);
    Match(TokenType.Semicolon);
    return new ContinueLoopExpression();
  }

  /// <summary>
  /// break_statement = "break", ";".
  /// </summary>
  private Expression ParseBreakExpr()
  {
    Match(TokenType.Break);
    Match(TokenType.Semicolon);
    return new BreakLoopExpression();
  }

  /// <summary>
  /// while_statement = "while", "(", expression, ")", block ;.
  /// </summary>
  private Expression ParseWhileExpr()
  {
    Match(TokenType.While);
    Match(TokenType.OpenParenthesis);
    Expression condition = ParseExpr();
    Match(TokenType.CloseParenthesis);
    Expression block = ParseBlock();

    return new WhileLoopExpression(condition, block);
  }

  /// <summary>
  /// if_statement = "if", "(", expression, ")", block, { elif_clause }, [ else_clause ] ;.
  /// elif_clause =  "elif", "(", expression, ")", block ;.
  /// else_clause = "else", block ;.
  /// </summary>
  private Expression ParseIfExpression()
  {
    Match(TokenType.If);
    Match(TokenType.OpenParenthesis);
    Expression expr = ParseExpr();
    Match(TokenType.CloseParenthesis);
    Expression thenBranch = ParseBlock();

    Expression? elseBranch = null;

    List<(Expression condition, Expression block)> elifBranches = new List<(Expression, Expression)>();
    while (tokens.Peek().Type == TokenType.Elif)
    {
      tokens.Advance();
      Match(TokenType.OpenParenthesis);
      Expression elifCondition = ParseExpr();
      Match(TokenType.CloseParenthesis);
      Expression elifBlock = ParseBlock();

      elifBranches.Add((elifCondition, elifBlock));
    }

    if (tokens.Peek().Type == TokenType.Else)
    {
      tokens.Advance();
      elseBranch = ParseBlock();
    }

    for (int i = elifBranches.Count - 1; i >= 0; i--)
    {
      Expression cond = elifBranches[i].Item1;
      Expression block = elifBranches[i].Item2;
      elseBranch = new IfElseExpression(cond, block, elseBranch);
    }

    return new IfElseExpression(expr, thenBranch, elseBranch);
  }

  /// <summary>
  /// for_statement = "for", "(", for_init, [ expression ], ";", [ for_update ], ")", block ;.
  /// for_init = variable_declaration | expression_statement | empty_statement ;.
  /// for_update = expression | empty_statement ;.
  /// </summary>
  private Expression ParseForExpr()
  {
    Match(TokenType.For);
    Match(TokenType.OpenParenthesis);
    string name;
    Expression forInit;

    switch (tokens.Peek().Type)
    {
      case TokenType.Let:
        tokens.Advance();
        name = tokens.Peek().Value!.ToString();
        tokens.Advance();
        Match(TokenType.ColonTypeIndication);
        Match(TokenType.Int);
        Match(TokenType.Assignment);

        forInit = ParseExpr();
        break;
      default:
        throw new Exception("");
    }

    Match(TokenType.Semicolon);
    Expression endCondition = ParseExpr();
    Match(TokenType.Semicolon);
    Expression stepValue = ParseExpr();
    Match(TokenType.CloseParenthesis);
    Expression body = ParseBlock();

    return new ForLoopExpression(name, forInit, endCondition, stepValue, body);
  }

  /// <summary>
  /// return_statement = "return", [ expression ], ";".
  /// </summary>
  private Expression ParseReturnStatement()
  {
    Match(TokenType.Return);

    Expression? expr = null;
    if (tokens.Peek().Type != TokenType.Semicolon)
    {
      expr = ParseExpr();
    }

    Match(TokenType.Semicolon);
    return new ReturnExpression(expr);
  }

  /// <summary>
  /// "func", identifier, ":", type, "(", [ parameter_list ], ")", block ;.
  /// </summary>
  private AstNode ParseFunctionDeclaration()
  {
    Match(TokenType.Func);
    string name = tokens.Peek().Value!.ToString();
    tokens.Advance();
    Match(TokenType.ColonTypeIndication);
    string type = GetValueType();
    Match(TokenType.OpenParenthesis);
    List<ParameterDeclaration> parameters = ParseParameterList();
    Match(TokenType.CloseParenthesis);
    Expression value = ParseBlock();
    return new FunctionDeclaration(name, parameters, type, value);
  }

  /// <summary>
  /// parameter_list = parameter, { ",", parameter } ;.
  /// </summary>
  private List<ParameterDeclaration> ParseParameterList()
  {
    List<ParameterDeclaration> parameters = [];

    while (tokens.Peek().Type != TokenType.CloseParenthesis)
    {
      string parameter = Match(TokenType.Identifier).Value!.ToString();
      Match(TokenType.ColonTypeIndication);
      string type = GetValueType();
      parameters.Add(new ParameterDeclaration(parameter, type));

      if (tokens.Peek().Type != TokenType.Comma)
      {
        break;
      }

      tokens.Advance();
    }

    return parameters;
  }

  /// <summary>
  /// Парсинг переменных
  /// value_declaration = variable_declaration | constant_declaration;.
  /// </summary>
  private Declaration ParseValueDeclaration()
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
  private Declaration ParseVariableDeclaration()
  {
    tokens.Advance();
    string name = tokens.Peek().Value!.ToString();

    if (ReservedNames.All.Contains(name))
    {
      throw new Exception($"'{name}' is a reserved name");
    }

    tokens.Advance();
    Match(TokenType.ColonTypeIndication);

    string type = GetValueType();
    if (type == "void")
    {
      throw new Exception("Constant can't be a void");
    }

    Expression? value = null;

    if (tokens.Peek().Type == TokenType.Assignment)
    {
      tokens.Advance();
      value = ParseExpr();
    }

    return new VariableDeclaration(name, type, value);
  }

  /// <summary>
  /// Parsing AssignableExpr
  /// assignable_expr, "=", expression, ";" ;.
  /// </summary>
  private Expression ParseAssignableExpr(string name)
  {
    Match(TokenType.Assignment);
    Expression left = new VariableExpression(name);
    Expression right = ParseExpr();
    return new AssignmentExpression(left, right);
  }

  /// <summary>
  /// constant_declaration = "const", identifier, ":", type "=", expression, ";";.
  /// </summary>
  private Declaration ParseConstantDeclaration()
  {
    tokens.Advance();
    Token t = tokens.Peek();

    if (t.Type != TokenType.Identifier || t.Value == null)
    {
      throw new UnexpectedLexemeException(TokenType.Identifier, t);
    }

    string name = tokens.Peek().Value!.ToString();

    if (ReservedNames.All.Contains(name))
    {
      throw new Exception($"'{name}' is a reserved name");
    }

    tokens.Advance();
    Match(TokenType.ColonTypeIndication);
    string type = GetValueType();
    if (type == "void")
    {
      throw new Exception("Constant can't be a void");
    }

    Match(TokenType.Assignment);
    Expression value = ParseExpr();

    return new ConstantDeclaration(name, type, value);
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
        case TokenType.OpenBlockComments:
          value = ParsePostfixOperator();
          continue;

        default:
          return value;
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

      case TokenType.NumberLiteral:
      case TokenType.StringLiteral:
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
      case TokenType.SingleLineComment:
        tokens.Advance();
        return new EmptyExpression();
      case TokenType.OpenBlockComments:
        Match(TokenType.CloseBlockComments);
        tokens.Advance();
        return new EmptyExpression();
      default:
        throw new UnexpectedLexemeException(t.Type, t);
    }
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

    Value value = environment.Read();

    ValueType type = value switch
    {
      _ when value.IsInt() => ValueType.Int,
      _ when value.IsFloat() => ValueType.Float,
      _ when value.IsBool() => ValueType.Bool,
      _ when value.IsString() => ValueType.String,
      _ => throw new InvalidOperationException($"Unknown value type: {value}")
    };

    return new LiteralExpression(type, value);
  }

  /// <summary>
  /// Парсинг вывода
  /// print_expr = "print", "(", [expression_list], ")" ;.
  /// </summary>
  private Expression ParsePrintExpr()
  {
    tokens.Advance();
    Match(TokenType.OpenParenthesis);
    List<Expression> values = ParseExpressionList();
    Match(TokenType.CloseParenthesis);
    return new PrintExpression(values);
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
      TokenType.Euler => new LiteralExpression(ValueType.Float, new Value(2.7182818f)),
      TokenType.Pi => new LiteralExpression(ValueType.Float, new Value(3.1415927f)),
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
        {
          tokens.Advance();
          string text = t.Value!.ToString();

          if (text.Contains('.'))
          {
            float f = float.Parse(text, CultureInfo.InvariantCulture);
            return new LiteralExpression(ValueType.Float, new Value(f));
          }
          else
          {
            int i = int.Parse(text, CultureInfo.InvariantCulture);
            return new LiteralExpression(ValueType.Float, new Value(i));
          }
        }

      case TokenType.StringLiteral:
        {
          tokens.Advance();
          string text = t.Value!.ToString();

          return new LiteralExpression(ValueType.String, new Value(text));
        }

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
      TokenType.True => new LiteralExpression(ValueType.Bool, new Value(true)),
      TokenType.False => new LiteralExpression(ValueType.Bool, new Value(false)),
      _ => throw new UnexpectedLexemeException(t.Type, t),
    };
  }

  /// <summary>
  /// Парсинг постфиксных операторов.
  /// postfix_operator = function_call | index_access | "++" | "--" ;.
  /// </summary>
  private Expression ParsePostfixOperator()
  {
    Token t = tokens.Peek();

    return t.Type switch
    {
      _ => throw new UnexpectedLexemeException(t.Type, t),
    };
  }

  /// <summary>
  /// Парсинг вызова функции
  /// function_call = "(", [ argument_list ], ")" ;.
  /// </summary>
  private Expression ParseFunctionCall(string name)
  {
    Match(TokenType.OpenParenthesis);
    List<Expression> args = [];
    if (tokens.Peek().Type != TokenType.CloseParenthesis)
    {
      args = ParseArgumentList();
    }

    Match(TokenType.CloseParenthesis);

    return new FunctionCallExpression(name, args);
  }

  /// <summary>
  /// argument_list = expression, { ",", expression } ;.
  /// </summary>
  private List<Expression> ParseArgumentList()
  {
    List<Expression> values = new List<Expression> { ParseExpr() };
    while (tokens.Peek().Type == TokenType.Comma)
    {
      tokens.Advance();
      values.Add(ParseExpr());
    }

    return values;
  }

  private string GetValueType()
  {
    Token t = tokens.Peek();
    tokens.Advance();

    switch (t.Type)
    {
      case TokenType.Int:
        return "int";
      case TokenType.Float:
        return "float";
      case TokenType.Str:
        return "string";
      case TokenType.Bool:
        return "bool";
      case TokenType.Void:
        return "void";
      default:
        throw new UnexpectedLexemeException(t.Type, t);
    }
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
