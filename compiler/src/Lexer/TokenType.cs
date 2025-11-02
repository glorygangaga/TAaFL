namespace Lexer;

public enum TokenType
{
  /// <summary>
  /// Идентификатор (имя переменной, функции, структуры)
  /// </summary>
  Identifier,

  /// <summary>
  /// Объявление константы const
  /// </summary>
  Const,

  /// <summary>
  /// Объявление переменной let
  /// </summary>
  Let,

  /// <summary>
  /// Тип данных str
  /// </summary>
  Str,

  /// <summary>
  /// Строковый литерал
  /// </summary>
  StringLiteral,

  /// <summary>
  /// Тип данных int
  /// </summary>
  Int,

  /// <summary>
  /// Тип данных float
  /// </summary>
  Float,

  /// <summary>
  /// Числовой литерал (int или float)
  /// </summary>
  NumberLiteral,

  /// <summary>
  /// Тип данных bool
  /// </summary>
  Bool,

  /// <summary>
  /// Тип данных void
  /// </summary>
  Void,

  /// <summary>
  /// Ветвление и циклы if
  /// </summary>
  If,

  /// <summary>
  /// Ветвление и циклы Else
  /// </summary>
  Else,

  /// <summary>
  /// Ветвление и циклы For
  /// </summary>
  For,

  /// <summary>
  /// Ветвление и циклы While
  /// </summary>
  While,

  /// <summary>
  /// Ключевое слово func
  /// </summary>
  Func,

  /// <summary>
  /// Ключевое слово return
  /// </summary>
  Return,

  /// <summary>
  /// Ключевое слово struct
  /// </summary>
  Struct,

  /// <summary>
  /// Ключевое слово import
  /// </summary>
  Import,

  /// <summary>
  /// Ключевое слово input
  /// </summary>
  Input,

  /// <summary>
  /// Ключевое слово true
  /// </summary>
  True,

  /// <summary>
  /// Ключевое слово false
  /// </summary>
  False,

  /// <summary>
  /// Ключевое слово print
  /// </summary>
  Print,

  /// <summary>
  /// Оператор присваивания равно "=".
  /// </summary>
  Assignment,

  /// <summary>
  /// Оператор сравнения равно "==".
  /// </summary>
  Equal,

  /// <summary>
  /// Оператор сравнения не равно "!=".
  /// </summary>
  NotEqual,

  /// <summary>
  ///  Оператор сравнения меньше "<".
  /// </summary>
  LessThan,

  /// <summary>
  ///  Оператор сравнения меньше или равно "<=".
  /// </summary>
  LessThanOrEqual,

  /// <summary>
  ///  Оператор сравнения больше ">".
  /// </summary>
  GreaterThan,

  /// <summary>
  ///  Оператор сравнения больше или равно ">=".
  /// </summary>
  GreaterThanOrEqual,

  /// <summary>
  /// Арифметический оператор плюс "+"
  /// </summary>
  Plus,

  /// <summary>
  /// Арифметический оператор инкремента "++"
  /// </summary>
  Increment,

  /// <summary>
  /// Арифметический оператор декремента "--"
  /// </summary>
  Decrement,

  /// <summary>
  /// Арифметический оператор минус "-"
  /// </summary>
  Minus,

  /// <summary>
  /// Арифметический оператор умножения "*"
  /// </summary>
  Multiplication,

  /// <summary>
  /// Арифметический оператор деления "/"
  /// </summary>
  Division,

  /// <summary>
  /// Арифметический целочисленный оператор деления "//"
  /// </summary>
  IntegerDivision,

  /// <summary>
  /// Арифметический оператор остаток "%"
  /// </summary>
  Remainder,

  /// <summary>
  /// Арифметический оператор возведения в степень "**"
  /// </summary>
  Exponentiation,

  /// <summary>
  /// Логический оператор "и"
  /// </summary>
  And,

  /// <summary>
  /// Логический оператор "или"
  /// </summary>
  Or,

  /// <summary>
  /// Логический оператор "не"
  /// </summary>
  Not,

  /// <summary>
  /// Оператор указания типа (разделитель типа) ":"
  /// </summary>
  ColonTypeIndication,

  /// <summary>
  /// Оператор доступа к полю структуры "."
  /// </summary>
  DotFieldAccess,

  /// <summary>
  ///  Открывающая круглая скобка '('.
  /// </summary>
  OpenParenthesis,

  /// <summary>
  ///  Закрывающая круглая скобка ')'.
  /// </summary>
  CloseParenthesis,

  /// <summary>
  /// Открывающая квадратная скобка '['.
  /// </summary>
  OpenSquareBracket,

  /// <summary>
  /// Закрывающая квадратная скобка ']'.
  /// </summary>
  CloseSquareBracket,

  /// <summary>
  /// Открывающая фигурная скобка '{'.
  /// </summary>
  OpenCurlyBrace,

  /// <summary>
  /// Закрывающая фигурная скобка '}'.
  /// </summary>
  CloseCurlyBracket,

  /// <summary>
  /// Открывающие многострочные комментарии '/*'
  /// </summary>
  OpenBlockComments,

  /// <summary>
  /// Закрывающие многострочные комментарии '*/'
  /// </summary>
  CloseBlockComments,

  /// <summary>
  ///  Разделитель элементов ','
  /// </summary>
  Comma,

  /// <summary>
  ///  Конец инструкции ';'
  /// </summary>
  Semicolon,

  /// <summary>
  /// Строка '"'
  /// </summary>
  DoubleQuote,

  /// <summary>
  /// Однострочные комментарии '///'
  /// </summary>
  SingleLineComment,

  /// <summary>
  /// Константа 3.14159265358
  /// </summary>
  Pi,

  /// <summary>
  /// Константа 2.71828182846
  /// </summary>
  Euler,

  /// <summary>
  /// модуль числа
  /// </summary>
  Abs,

  /// <summary>
  /// минимальное значение
  /// </summary>
  Min,

  /// <summary>
  /// максимальное значение
  /// </summary>
  Max,

  /// <summary>
  /// возведение в степень
  /// </summary>
  Pow,

  /// <summary>
  /// округление до ближайшего целого
  /// </summary>
  Round,

  /// <summary>
  /// округление вверх
  /// </summary>
  Ceil,

  /// <summary>
  /// округление вниз
  /// </summary>
  Floor,

  /// <summary>
  ///  Конец файла.
  /// </summary>
  EndOfFile,

  /// <summary>
  ///  Недопустимая лексема.
  /// </summary>
  Error,
}
