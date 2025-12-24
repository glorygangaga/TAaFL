using System.Runtime.CompilerServices;

namespace Runtime;

public class ValueType
{
  public static readonly ValueType Void = new("void");
  public static readonly ValueType Bool = new("bool");
  public static readonly ValueType String = new("string");
  public static readonly ValueType Float = new("float");
  public static readonly ValueType Int = new("int");

  private readonly string name;

  protected ValueType(string name)
  {
    this.name = name;
  }

  public static bool operator ==(ValueType a, ValueType b) => a.Equals(b);

  public static bool operator !=(ValueType a, ValueType b) => !a.Equals(b);

  public override bool Equals(object? obj)
  {
    return ReferenceEquals(this, obj);
  }

  public override int GetHashCode()
  {
    return RuntimeHelpers.GetHashCode(this);
  }

  public override string ToString()
  {
    return name;
  }
}