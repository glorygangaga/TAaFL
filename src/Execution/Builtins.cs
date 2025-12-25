using Ast.Declarations;

using ValueType = Runtime.ValueType;

namespace Execution;

/// <summary>
/// Объект, предоставляющий доступ к встроенным символам языка.
/// </summary>
public class Builtins
{
  public Builtins()
  {
    Functions =
    [
      new("abs",
        [ new("x", ValueType.Float)],
        ValueType.Float,
        args => BuiltinFunctions.Instance.Invoke("abs", args)
      ),

      new("abs",
        [ new("x", ValueType.Int)],
        ValueType.Int,
        args => BuiltinFunctions.Instance.Invoke("abs", args)
      ),

      new("min",
        [ new("x", ValueType.Float), new("y", ValueType.Float)],
        ValueType.Float,
        args => BuiltinFunctions.Instance.Invoke("min", args)
      ),

      new("min",
        [ new("x", ValueType.Int), new("y", ValueType.Int)],
        ValueType.Int,
        args => BuiltinFunctions.Instance.Invoke("min", args)
      ),

      new("max",
        [ new("x", ValueType.Float), new("y", ValueType.Float)],
        ValueType.Float,
        args => BuiltinFunctions.Instance.Invoke("max", args)
      ),

      new("max",
        [ new("x", ValueType.Int), new("y", ValueType.Int)],
        ValueType.Int,
        args => BuiltinFunctions.Instance.Invoke("max", args)
      ),

      new("pow",
        [ new("a", ValueType.Float), new("b", ValueType.Float)],
        ValueType.Float,
        args => BuiltinFunctions.Instance.Invoke("pow", args)
      ),

      new("pow",
        [ new("a", ValueType.Int), new("b", ValueType.Int)],
        ValueType.Float,
        args => BuiltinFunctions.Instance.Invoke("pow", args)
      ),

      new("round",
        [ new("x", ValueType.Float)],
        ValueType.Float,
        args => BuiltinFunctions.Instance.Invoke("round", args)
      ),

      new("round",
        [ new("x", ValueType.Int)],
        ValueType.Float,
        args => BuiltinFunctions.Instance.Invoke("round", args)
      ),

      new("ceil",
        [ new("x", ValueType.Float)],
        ValueType.Float,
        args => BuiltinFunctions.Instance.Invoke("ceil", args)
      ),

      new("ceil",
        [ new("x", ValueType.Int)],
        ValueType.Float,
        args => BuiltinFunctions.Instance.Invoke("ceil", args)
      ),

      new("floor",
        [ new("x", ValueType.Float)],
        ValueType.Float,
        args => BuiltinFunctions.Instance.Invoke("floor", args)
      ),

      new("floor",
        [ new("x", ValueType.Int)],
        ValueType.Float,
        args => BuiltinFunctions.Instance.Invoke("floor", args)
      ),

      new("length",
        [ new("s", ValueType.String)],
        ValueType.Int,
        args => BuiltinFunctions.Instance.Invoke("length", args)
      ),

      new("contains",
        [ new("s", ValueType.String), new("sub", ValueType.String)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("contains", args)
      ),

      new("startsWith",
        [ new("s", ValueType.String), new("prefix", ValueType.String)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("startsWith", args)
      ),

      new("endsWith",
        [ new("s", ValueType.String), new("suffix", ValueType.String)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("endsWith", args)
      ),

      new("toLower",
        [ new("s", ValueType.String)],
        ValueType.String,
        args => BuiltinFunctions.Instance.Invoke("toLower", args)
      ),

      new("toUpper",
        [ new("s", ValueType.String)],
        ValueType.String,
        args => BuiltinFunctions.Instance.Invoke("toUpper", args)
      ),

      new("trim",
        [ new("s", ValueType.String)],
        ValueType.String,
        args => BuiltinFunctions.Instance.Invoke("trim", args)
      ),

      new("indexOf",
        [ new("s", ValueType.String), new("sub", ValueType.String)],
        ValueType.Int,
        args => BuiltinFunctions.Instance.Invoke("indexOf", args)
      ),

      new("lastIndexOf",
        [ new("s", ValueType.String), new("sub", ValueType.String)],
        ValueType.Int,
        args => BuiltinFunctions.Instance.Invoke("lastIndexOf", args)
      ),

      new("replace",
        [new("s", ValueType.String), new("old", ValueType.String), new("nw", ValueType.String)],
        ValueType.String,
        args => BuiltinFunctions.Instance.Invoke("replace", args)
      ),

      new("toString",
        [ new("x", ValueType.String)],
        ValueType.String,
        args => BuiltinFunctions.Instance.Invoke("toString", args)
      ),

      new("toString",
        [ new("x", ValueType.Int)],
        ValueType.String,
        args => BuiltinFunctions.Instance.Invoke("toString", args)
      ),

      new("toString",
        [ new("x", ValueType.Float)],
        ValueType.String,
        args => BuiltinFunctions.Instance.Invoke("toString", args)
      ),

      new("toString",
        [ new("x", ValueType.Bool)],
        ValueType.String,
        args => BuiltinFunctions.Instance.Invoke("toString", args)
      ),

      new("toInt",
        [ new("x", ValueType.Int)],
        ValueType.Int,
        args => BuiltinFunctions.Instance.Invoke("toInt", args)
      ),

      new("toInt",
        [ new("x", ValueType.Float)],
        ValueType.Int,
        args => BuiltinFunctions.Instance.Invoke("toInt", args)
      ),

      new("toInt",
        [ new("x", ValueType.Bool)],
        ValueType.Int,
        args => BuiltinFunctions.Instance.Invoke("toInt", args)
      ),

      new("toInt",
        [ new("x", ValueType.String)],
        ValueType.Int,
        args => BuiltinFunctions.Instance.Invoke("toInt", args)
      ),

      new("toFloat",
        [ new("x", ValueType.Float)],
        ValueType.Float,
        args => BuiltinFunctions.Instance.Invoke("toFloat", args)
      ),

      new("toFloat",
        [ new("x", ValueType.Int)],
        ValueType.Float,
        args => BuiltinFunctions.Instance.Invoke("toFloat", args)
      ),

      new("toFloat",
        [ new("x", ValueType.String)],
        ValueType.Float,
        args => BuiltinFunctions.Instance.Invoke("toFloat", args)
      ),

      new("toFloat",
        [ new("x", ValueType.Bool)],
        ValueType.Float,
        args => BuiltinFunctions.Instance.Invoke("toFloat", args)
      ),

      new("toBool",
        [ new("x", ValueType.Bool)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("toBool", args)
      ),

      new("toBool",
        [ new("x", ValueType.Int)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("toBool", args)
      ),

      new("toBool",
        [ new("x", ValueType.String)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("toBool", args)
      ),

      new("toBool",
        [ new("x", ValueType.Float)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("toBool", args)
      ),

      new("isInt",
        [ new("x", ValueType.Int)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isInt", args)
      ),

      new("isInt",
        [ new("x", ValueType.Float)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isInt", args)
      ),

      new("isInt",
        [ new("x", ValueType.Bool)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isInt", args)
      ),

      new("isInt",
        [ new("x", ValueType.String)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isInt", args)
      ),

      new("isFloat",
        [ new("x", ValueType.Float)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isFloat", args)
      ),

      new("isFloat",
        [ new("x", ValueType.Bool)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isFloat", args)
      ),

      new("isFloat",
        [ new("x", ValueType.String)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isFloat", args)
      ),

      new("isFloat",
        [ new("x", ValueType.Int)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isFloat", args)
      ),

      new("isBool",
        [ new("x", ValueType.Bool)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isBool", args)
      ),

      new("isBool",
        [ new("x", ValueType.String)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isBool", args)
      ),

      new("isBool",
        [ new("x", ValueType.Int)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isBool", args)
      ),

      new("isBool",
        [ new("x", ValueType.Float)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isBool", args)
      ),

      new("isStr",
        [ new("x", ValueType.String)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isStr", args)
      ),

      new("isStr",
        [ new("x", ValueType.Bool)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isStr", args)
      ),

      new("isStr",
        [ new("x", ValueType.Int)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isStr", args)
      ),

      new("isStr",
        [ new("x", ValueType.Float)],
        ValueType.Bool,
        args => BuiltinFunctions.Instance.Invoke("isStr", args)
      ),
    ];

    Types =
    [
      new("int", ValueType.Int),
      new("string", ValueType.String),
      new("bool", ValueType.Bool),
      new("float", ValueType.Float),
      new("void", ValueType.Void),
    ];
  }

  /// <summary>
  /// Список встроенных функций языка.
  /// </summary>
  public IReadOnlyList<BuiltinFunction> Functions { get; }

  /// <summary>
  /// Список встроенных типов языка.
  /// </summary>
  public IReadOnlyList<BuiltinType> Types { get; }
}