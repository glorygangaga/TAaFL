using Runtime;

namespace Execution;

public sealed class BuiltinFunctions
{
  private readonly Dictionary<string, Func<List<Value>, Value>> functions = new()
  {
    { "abs", Abs },
    { "min", Min },
    { "max", Max },
    { "pow", Pow },
    { "round", Round },
    { "ceil", Ceil },
    { "floor", Floor },
    { "length", Length },
    { "substring", Substring },
    { "contains", Contains },
    { "startsWith", StartsWith },
    { "endsWith", EndsWith },
    { "toLower", ToLower },
    { "toUpper", ToUpper },
    { "trim", Trim },
    { "indexOf", IndexOf },
    { "lastIndexOf", LastIndexOf },
    { "replace", Replace },
    { "toString", ToString },
    { "toInt", ToInt },
    { "toFloat", ToFloat },
    { "toBool", ToBool },
    { "isInt", IsInt },
    { "isFloat", IsFloat },
    { "isBool", IsBool },
    { "isStr", IsStr },
  };

  private static readonly BuiltinFunctions InstanceValue = new();

  public static BuiltinFunctions Instance => InstanceValue;

  public Value Invoke(string name, List<Value> arguments)
  {
    if (!functions.TryGetValue(name, out Func<List<Value>, Value>? function))
    {
      throw new ArgumentException($"Unknown builtin function '{name}'");
    }

    return function(arguments);
  }

  public bool IsBuiltin(string name)
  {
    return functions.ContainsKey(name);
  }

  private static float AsNumber(Value value)
  {
    if (value.IsInt())
    {
      return value.AsInt();
    }

    if (value.IsFloat())
    {
      return value.AsFloat();
    }

    throw new InvalidOperationException($"Expected number, got {value}");
  }

  private static void ExpectCount(List<Value> args, int count, string name)
  {
    if (args.Count != count)
    {
      throw new ArgumentException(
        $"Function '{name}' expects {count} arguments, but got {args.Count}");
    }
  }

  private static Value Abs(List<Value> args)
  {
    ExpectCount(args, 1, "abs");
    return new Value(Math.Abs(AsNumber(args[0])));
  }

  private static Value Min(List<Value> args)
  {
    if (args.Count == 0)
    {
      throw new ArgumentException("Function 'min' expects at least one argument");
    }

    float result = AsNumber(args[0]);

    for (int i = 1; i < args.Count; i++)
    {
      result = Math.Min(result, AsNumber(args[i]));
    }

    return new Value(result);
  }

  private static Value Max(List<Value> args)
  {
    if (args.Count == 0)
    {
      throw new ArgumentException("Function 'max' expects at least one argument");
    }

    float result = AsNumber(args[0]);

    for (int i = 1; i < args.Count; i++)
    {
      result = Math.Max(result, AsNumber(args[i]));
    }

    return new Value(result);
  }

  private static Value Pow(List<Value> args)
  {
    ExpectCount(args, 2, "pow");

    float a = AsNumber(args[0]);
    float b = AsNumber(args[1]);

    return new Value((float)Math.Pow(a, b));
  }

  private static Value Ceil(List<Value> args)
  {
    ExpectCount(args, 1, "ceil");
    return new Value((float)Math.Ceiling(AsNumber(args[0])));
  }

  private static Value Floor(List<Value> args)
  {
    ExpectCount(args, 1, "floor");
    return new Value((float)Math.Floor(AsNumber(args[0])));
  }

  private static Value Round(List<Value> args)
  {
    ExpectCount(args, 1, "round");
    return new Value((float)Math.Round(AsNumber(args[0])));
  }

  private static Value Length(List<Value> args)
  {
    ExpectCount(args, 1, "length");
    return new Value(args[0].AsString().Length);
  }

  private static Value Substring(List<Value> args)
  {
    ExpectCount(args, 3, "substring");
    return new Value(args[0].AsString().Substring(args[1].AsInt(), args[2].AsInt()));
  }

  private static Value Contains(List<Value> args)
  {
    ExpectCount(args, 2, "contains");
    return new Value(args[0].AsString().Contains(args[1].AsString()));
  }

  private static Value StartsWith(List<Value> args)
  {
    ExpectCount(args, 2, "startsWith");
    return new Value(args[0].AsString().StartsWith(args[1].AsString()));
  }

  private static Value EndsWith(List<Value> args)
  {
    ExpectCount(args, 2, "endsWith");
    return new Value(args[0].AsString().EndsWith(args[1].AsString()));
  }

  private static Value ToLower(List<Value> args)
  {
    ExpectCount(args, 1, "toLower");
    return new Value(args[0].AsString().ToLower());
  }

  private static Value ToUpper(List<Value> args)
  {
    ExpectCount(args, 1, "toUpper");
    return new Value(args[0].AsString().ToUpper());
  }

  private static Value Trim(List<Value> args)
  {
    ExpectCount(args, 1, "Trim");
    return new Value(args[0].AsString().Trim());
  }

  private static Value IndexOf(List<Value> args)
  {
    ExpectCount(args, 2, "indexOf");
    return new Value(args[0].AsString().IndexOf(args[1].AsString()));
  }

  private static Value LastIndexOf(List<Value> args)
  {
    ExpectCount(args, 2, "lastIndexOf");
    return new Value(args[0].AsString().LastIndexOf(args[1].AsString()));
  }

  private static Value Replace(List<Value> args)
  {
    ExpectCount(args, 3, "replace");
    return new Value(args[0].AsString().Replace(args[1].AsString(), args[2].AsString()));
  }

  private static Value ToString(List<Value> args)
  {
    ExpectCount(args, 1, "toString");

    return args[0] switch
    {
      { } when args[0].IsBool() => new Value(args[0].AsBool().ToString()),
      { } when args[0].IsInt() => new Value(args[0].AsInt().ToString()),
      { } when args[0].IsFloat() => new Value(args[0].AsFloat().ToString()),
      { } when args[0].IsString() => new Value(args[0].AsString()),
      _ => throw new InvalidOperationException($"Cannot convert {args[0]} to string")
    };
  }

  private static Value ToInt(List<Value> args)
  {
    ExpectCount(args, 1, "toInt");

    return args[0] switch
    {
      { } when args[0].IsInt() => new Value(args[0].AsInt()),
      { } when args[0].IsFloat() => new Value((int)args[0].AsFloat()),
      { } when args[0].IsString() => new Value(int.Parse(args[0].AsString())),
      _ => throw new InvalidOperationException($"Cannot convert {args[0]} to string")
    };
  }

  private static Value ToFloat(List<Value> args)
  {
    ExpectCount(args, 1, "toFloat");

    return args[0] switch
    {
      { } when args[0].IsInt() => new Value((float)args[0].AsInt()),
      { } when args[0].IsFloat() => new Value(args[0].AsFloat()),
      { } when args[0].IsString() => new Value(float.Parse(args[0].AsString())),
      _ => throw new InvalidOperationException($"Cannot convert {args[0]} to string")
    };
  }

  private static Value ToBool(List<Value> args)
  {
    ExpectCount(args, 1, "toBool");

    return args[0] switch
    {
      { } when args[0].IsBool() => new Value(args[0].AsBool()),
      { } when args[0].IsInt() => new Value(args[0].AsInt() != 0),
      { } when args[0].IsFloat() => new Value(args[0].AsFloat() != 0),
      { } when args[0].IsString() => new Value(args[0].AsString().Length != 0),
      _ => throw new InvalidOperationException($"Cannot convert {args[0]} to string")
    };
  }

  private static Value IsInt(List<Value> args)
  {
    ExpectCount(args, 1, "isInt");
    return new Value(args[0].IsInt());
  }

  private static Value IsBool(List<Value> args)
  {
    ExpectCount(args, 1, "isBool");
    return new Value(args[0].IsBool());
  }

  private static Value IsFloat(List<Value> args)
  {
    ExpectCount(args, 1, "isFloat");
    return new Value(args[0].IsFloat());
  }

  private static Value IsStr(List<Value> args)
  {
    ExpectCount(args, 1, "isStr");
    return new Value(args[0].IsString());
  }
}
