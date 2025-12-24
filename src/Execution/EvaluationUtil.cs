using Ast.Expressions;

using Runtime;

namespace Execution;

public static class EvaluationUtil
{
  public static Value ApplyBinaryOperation(
        BinaryOperation operation, Func<Value> evaluateLeft, Func<Value> evaluateRight
    )
  {
    return operation switch
    {
      BinaryOperation.Plus => ApplyArithmeticOperation(
          evaluateLeft,
          evaluateRight,
          (i1, i2) => i1 + i2
      ),
      BinaryOperation.Minus => ApplyArithmeticOperation(
          evaluateLeft,
          evaluateRight,
          (i1, i2) => i1 - i2
      ),
      BinaryOperation.Multiplication => ApplyArithmeticOperation(
          evaluateLeft,
          evaluateRight,
          (i1, i2) => i1 * i2
      ),
      BinaryOperation.Division => ApplyArithmeticOperation(
          evaluateLeft,
          evaluateRight,
          (i1, i2) => i1 / i2
      ),
      BinaryOperation.Exponentiation => ApplyArithmeticOperation(
          evaluateLeft,
          evaluateRight,
          (i1, i2) => (decimal)Math.Pow((double)i1, (double)i2)
      ),
      BinaryOperation.Remainder => ApplyArithmeticOperation(
          evaluateLeft,
          evaluateRight,
          (i1, i2) => i1 % i2
      ),
      BinaryOperation.IntegerDivision => ApplyArithmeticOperation(
          evaluateLeft,
          evaluateRight,
          (i1, i2) => (int)Math.Floor(i1 / i2)
      ),
      BinaryOperation.Equal => ApplyEqualityOperator(
          evaluateLeft,
          evaluateRight,
          (v1, v2) => v1.Equals(v2)
      ),
      BinaryOperation.NotEqual => ApplyEqualityOperator(
          evaluateLeft,
          evaluateRight,
          (v1, v2) => !v1.Equals(v2)
      ),
      BinaryOperation.LessThan => ApplyOrderingOperator(
          evaluateLeft,
          evaluateRight,
          (i1, i2) => i1 < i2,
          (s1, s2) => string.CompareOrdinal(s1, s2) < 0,
          (i1, i2) => i1 < i2
      ),
      BinaryOperation.GreaterThan => ApplyOrderingOperator(
          evaluateLeft,
          evaluateRight,
          (i1, i2) => i1 > i2,
          (s1, s2) => string.CompareOrdinal(s1, s2) > 0,
          (i1, i2) => i1 > i2
      ),
      BinaryOperation.LessThanOrEqual => ApplyOrderingOperator(
          evaluateLeft,
          evaluateRight,
          (i1, i2) => i1 <= i2,
          (s1, s2) => string.CompareOrdinal(s1, s2) <= 0,
          (i1, i2) => i1 <= i2
      ),
      BinaryOperation.GreaterThanOrEqual => ApplyOrderingOperator(
          evaluateLeft,
          evaluateRight,
          (i1, i2) => i1 >= i2,
          (s1, s2) => string.CompareOrdinal(s1, s2) >= 0,
          (i1, i2) => i1 >= i2
      ),
      BinaryOperation.Or => ApplyLogicalOr(
          evaluateLeft,
          evaluateRight
      ),
      BinaryOperation.And => ApplyLogicalAnd(
          evaluateLeft,
          evaluateRight
      ),
      _ => throw new NotImplementedException($"Unknown binary operation {operation}"),
    };
  }

  /// <summary>
  /// Выполняет арифметическую операцию, если оба операнда являются числами (int или float).
  /// Возвращает float, если хотя бы один из операндов float.
  /// Иначе бросает исключение.
  /// </summary>
  private static Value ApplyArithmeticOperation(
    Func<Value> evaluateLeft, Func<Value> evaluateRight, Func<decimal, decimal, decimal> operation
)
  {
    Value leftValue = evaluateLeft();
    Value rightValue = evaluateRight();

    if (leftValue.IsString() && rightValue.IsString())
    {
      return new Value(leftValue.AsString() + rightValue.AsString());
    }

    decimal left;
    decimal right;

    if (leftValue.IsInt() && rightValue.IsInt())
    {
      left = leftValue.AsInt();
      right = rightValue.AsInt();
    }
    else if ((leftValue.IsInt() || leftValue.IsFloat()) && (rightValue.IsInt() || rightValue.IsFloat()))
    {
      left = leftValue.IsInt() ? leftValue.AsInt() : (decimal)leftValue.AsFloat();
      right = rightValue.IsInt() ? rightValue.AsInt() : (decimal)rightValue.AsFloat();
    }
    else
    {
      throw new InvalidOperationException($"Operands must be numbers: {leftValue}, {rightValue}");
    }

    decimal result = operation(left, right);

    bool leftIsFloat = leftValue.IsFloat();
    bool rightIsFloat = rightValue.IsFloat();

    if (leftIsFloat || rightIsFloat)
    {
      return new Value((float)result);
    }

    // Если результат целый, возвращаем int, иначе float
    if (Math.Abs(result % 1) < 0.00001m)
    {
      return new Value((int)result);
    }

    return new Value((float)result);
  }

  /// <summary>
  /// Выполняет операцию сравнения значений на равенство / неравенство.
  /// </summary>
  private static Value ApplyEqualityOperator(
      Func<Value> evaluateLeft,
      Func<Value> evaluateRight,
      Func<Value, Value, bool> compare
  )
  {
    Value left = evaluateLeft();
    Value right = evaluateRight();

    bool result = compare(left, right);
    return new Value(result);
  }

  /// <summary>
  /// Сравнивает два операнда на относительный порядок, если они оба являются числами или строками.
  /// Иначе бросает исключение.
  /// </summary>
  private static Value ApplyOrderingOperator(
      Func<Value> evaluateLeft,
      Func<Value> evaluateRight,
      Func<int, int, bool> compareInts,
      Func<string, string, bool> compareStrings,
      Func<float, float, bool> compareFloats
  )
  {
    Value left = evaluateLeft();
    Value right = evaluateRight();

    if (left.IsInt() && right.IsInt())
    {
      bool result = compareInts(left.AsInt(), right.AsInt());
      return new Value(result);
    }

    if (left.IsString() && right.IsString())
    {
      bool result = compareStrings(left.AsString(), right.AsString());
      return new Value(result);
    }

    if (left.IsFloat() && right.IsFloat())
    {
      return new Value(compareFloats(left.AsFloat(), right.AsFloat()));
    }

    throw new InvalidOperationException($"Values are not comparable: {left} and {right}");
  }

  private static bool ToBool(Value value)
  {
    return value switch
    {
      { } when value.IsBool() => value.AsBool(),
      { } when value.IsInt() => value.AsInt() != 0,
      { } when value.IsFloat() => Math.Abs(value.AsFloat()) > 0.0001f,
      { } when value.IsString() => !string.IsNullOrEmpty(value.AsString()),
      _ => throw new InvalidOperationException($"Cannot convert {value} to boolean")
    };
  }

  /// <summary>
  /// Вычисляет логическое "ИЛИ".
  /// Реализует вычисление по короткой схеме (short-circuit evaluation).
  /// </summary>
  private static Value ApplyLogicalOr(Func<Value> evaluateLeft, Func<Value> evaluateRight)
  {
    if (ToBool(evaluateLeft()))
    {
      return new Value(true);
    }

    return new Value(ToBool(evaluateRight()));
  }

  /// <summary>
  /// Вычисляет логическое "И".
  /// Реализует вычисление по короткой схеме (short-circuit evaluation).
  /// </summary>
  private static Value ApplyLogicalAnd(Func<Value> evaluateLeft, Func<Value> evaluateRight)
  {
    if (!ToBool(evaluateLeft()))
    {
      return new Value(false);
    }

    return new Value(ToBool(evaluateRight()));
  }
}