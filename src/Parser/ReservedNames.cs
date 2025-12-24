namespace Parser;

public static class ReservedNames
{
  public static readonly HashSet<string> All =
  [
      "if", "elif", "else",
      "func", "return",
      "let", "const",
      "true", "false",

      "abs", "min", "max", "pow",
      "round", "ceil", "floor",

      "length", "substring", "contains", "startsWith",
      "endsWith", "toLower", "toUpper", "trim",
      "indexOf", "lastIndexOf", "replace",

      "toString", "toInt", "toFloat", "toBool",

      "isInt", "isFloat", "isBool", "isStr"
  ];
}
