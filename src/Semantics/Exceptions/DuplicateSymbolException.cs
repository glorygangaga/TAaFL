using Semantics.Symbols;

namespace Semantics.Exceptions;

/// <summary>
/// Исключение из-за повторного объявления символа с тем же именем.
/// </summary>
#pragma warning disable RCS1194
public class DuplicateSymbolException : Exception
{
  private DuplicateSymbolException(string message)
      : base(message)
  {
  }

  public static DuplicateSymbolException DuplicateVariableOrFunction(string name)
  {
    return new DuplicateSymbolException(
        $"The variable or function name {name} is already used in the current scope");
  }

  public static DuplicateSymbolException DuplicateType(string name)
  {
    return new DuplicateSymbolException($"The type name {name} is already used in the current scope");
  }

  public static DuplicateSymbolException DuplicateField(string name)
  {
    return new DuplicateSymbolException($"The field name {name} is already used in the record");
  }
}
#pragma warning restore RCS1194