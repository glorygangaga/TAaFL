using System.Globalization;

using ValueType = Runtime.ValueType;

namespace Runtime;

public class Value : IEquatable<Value>
{
  public const double Tolerance = 0.001d;
  public static readonly Value Void = new(VoidValue.Value);
  public static readonly Value Nil = new(NilValue.Value);

  private readonly object value;

  public Value(string value)
  {
    this.value = value;
  }

  public Value(int value)
  {
    this.value = value;
  }

  public Value(float value)
  {
    this.value = value;
  }

  public Value(bool value)
  {
    this.value = value;
  }

  private Value(object value)
  {
    this.value = value;
  }

  /// <summary>
  /// Определяет, является ли значение строкой.
  /// </summary>
  public bool IsString()
  {
    return value switch
    {
      string => true,
      _ => false,
    };
  }

  /// <summary>
  /// Возвращает значение как строку либо бросает исключение.
  /// </summary>
  public string AsString()
  {
    return value switch
    {
      string s => s,
      _ => throw new InvalidOperationException($"Value {value} is not a string"),
    };
  }

  /// <summary>
  /// Определяет, является ли значение целым числом.
  /// </summary>
  public bool IsInt()
  {
    return value switch
    {
      int => true,
      _ => false,
    };
  }

  /// <summary>
  /// Возвращает значение как целое число либо бросает исключение.
  /// </summary>
  public int AsInt()
  {
    return value switch
    {
      int i => i,
      _ => throw new InvalidOperationException($"Value {value} is not an integer"),
    };
  }

  /// <summary>
  /// Определяет, является ли значение вещественным числом.
  /// </summary>
  public bool IsFloat()
  {
    return value switch
    {
      float => true,
      _ => false,
    };
  }

  /// <summary>
  /// Возвращает значение как вещественное число либо бросает исключение.
  /// </summary>
  public float AsFloat()
  {
    return value switch
    {
      float i => i,
      _ => throw new InvalidOperationException($"Value {value} is not an integer"),
    };
  }

  /// <summary>
  /// Определяет, является ли значение булевым типом.
  /// </summary>
  public bool IsBool()
  {
    return value switch
    {
      bool => true,
      _ => false,
    };
  }

  /// <summary>
  /// Возвращает значение как булевый тип либо бросает исключение.
  /// </summary>
  public bool AsBool()
  {
    return value switch
    {
      bool i => i,
      _ => throw new InvalidOperationException($"Value {value} is not an integer"),
    };
  }

  /// <summary>
  /// Печатает значение для отладки.
  /// </summary>
  public override string ToString()
  {
    return value switch
    {
      string s => ValueUtil.EscapeStringValue(s),
      int i => i.ToString(CultureInfo.InvariantCulture),
      float f => f.ToString("G10", CultureInfo.InvariantCulture),
      bool b => b.ToString(CultureInfo.InstalledUICulture),
      VoidValue v => v.ToString(),
      NilValue v => v.ToString(),
      _ => throw new InvalidOperationException($"Unexpected value {value} of type {value.GetType()}"),
    };
  }

  public bool Equals(Value? other)
  {
    if (other is null)
    {
      return false;
    }

    return value switch
    {
      // Строки сравниваются посимвольно.
      string s => other.AsString() == s,

      // Числа сравниваются по значению.
      int i => other.AsInt() == i,

      // Вещественные числа сравниваются с погрешностью.
      float d => Math.Abs(other.AsFloat() - d) < Tolerance,

      // булевыt типы
      bool b => b == other.AsBool(),

      // Пустые значения всегда равны.
      VoidValue => true,

      // Несуществующая структура равна сама себе и не равна никаким другим.
      NilValue => other.value is NilValue,

      _ => throw new NotImplementedException(),
    };
  }

  public override bool Equals(object? obj)
  {
    return Equals(obj as Value);
  }

  public override int GetHashCode()
  {
    return value.GetHashCode();
  }
}