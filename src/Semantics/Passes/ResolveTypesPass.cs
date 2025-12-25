using Ast.Declarations;
using Ast.Expressions;

using Runtime;

using Semantics.Exceptions;
using Semantics.Helpers;

using ValueType = Runtime.ValueType;

namespace Semantics.Passes;

/// <summary>
/// Проход по AST для вычисления типов данных.
/// </summary>
/// <exception cref="TypeErrorException">Бросается при несоответствии типов данных в процессе вычисления типов.</exception>
public sealed class ResolveTypesPass : AbstractPass
{
  /// <summary>
  /// Литерал всегда имеет определённый тип.
  /// </summary>
  public override void Visit(LiteralExpression e)
  {
    base.Visit(e);
    e.ResultType = e.Type;
  }

  /// <summary>
  /// Выполняет проверки типов для бинарных операций:
  /// 1. Арифметические и логические операции выполняются над целыми числами и возвращают число.
  /// 2. Операции сравнения выполняются над двумя числами либо двумя строками и возвращают тот же тип.
  /// </summary>
  public override void Visit(BinaryOperationExpression e)
  {
    base.Visit(e);

    ValueType? resultType = GetBinaryOperationResultType(e.Operation, e.Left.ResultType, e.Right.ResultType);
    if (resultType is null)
    {
      throw new TypeErrorException(
          $"Binary operation {e.Operation} is not allowed for types {e.Left.ResultType} and {e.Right.ResultType}"
      );
    }

    e.ResultType = resultType;
  }

  public override void Visit(UnaryOperationExpression e)
  {
    base.Visit(e);

    ValueType valueType = e.Operand.ResultType;
    if (valueType != ValueType.Int && valueType != ValueType.Float && valueType != ValueType.Bool)
    {
      throw new TypeErrorException($"Unary operation is not allowed for type {valueType}");
    }

    e.ResultType = valueType;
  }

  public override void Visit(ReturnExpression e)
  {
    base.Visit(e);

    e.ResultType = e.Value != null
      ? e.Value.ResultType
      : ValueType.Void;
  }

  public override void Visit(VariableExpression e)
  {
    base.Visit(e);
    e.ResultType = e.Variable.ResultType;
  }

  public override void Visit(FunctionCallExpression e)
  {
    base.Visit(e);
    if (e.FunctionOverloads == null || e.FunctionOverloads.Count == 0)
    {
      throw new InvalidFunctionCallException($"Function {e.Name} not found");
    }

    AbstractFunctionDeclaration? matched = e.FunctionOverloads
    .FirstOrDefault(f =>
        f.Parameters.Count == (e.Arguments?.Count ?? 0) &&
        (f.Parameters.Count == 0 ||
         f.Parameters.Select(p => p.ResultType)
                     .SequenceEqual(e.Arguments!.Select(a => a.ResultType)))
    );

    if (matched == null)
    {
      throw new InvalidFunctionCallException($"No matching overload for function {e.Name}");
    }

    e.Function = matched;
    e.ResultType = matched.ResultType;
  }

  public override void Visit(VariableDeclaration d)
  {
    base.Visit(d);
    d.ResultType = d.InitialValue != null ? d.InitialValue.ResultType : ValueType.Void;
  }

  public override void Visit(ConstantDeclaration d)
  {
    base.Visit(d);
    d.ResultType = d.Value.ResultType;
  }

  public override void Visit(AssignmentExpression e)
  {
    base.Visit(e);
    e.ResultType = ValueType.Void;
  }

  public override void Visit(IfElseExpression e)
  {
    base.Visit(e);
    e.ResultType = e.ThenBranch.ResultType;
  }

  public override void Visit(WhileLoopExpression e)
  {
    base.Visit(e);

    e.ResultType = ValueType.Void;
  }

  public override void Visit(ForLoopExpression e)
  {
    base.Visit(e);
    e.ResultType = ValueType.Void;
  }

  public override void Visit(BreakLoopExpression e)
  {
    base.Visit(e);
    e.ResultType = ValueType.Void;
  }

  public override void Visit(PrintExpression e)
  {
    base.Visit(e);
    e.ResultType = ValueType.Void;
  }

  public override void Visit(DeclarationExpression e)
  {
    base.Visit(e);
    e.ResultType = ValueType.Void;
  }

  /// <summary>
  /// Вычисляет тип результата бинарной операции.
  /// Возвращает null, если бинарная операция не может быть выполнена с указанными типами.
  /// </summary>
  private static ValueType? GetBinaryOperationResultType(BinaryOperation operation, ValueType left, ValueType right)
  {
    switch (operation)
    {
      case BinaryOperation.Plus:
        if (left == ValueType.Int && right == ValueType.Int)
        {
          return ValueType.Int;
        }

        if (left == ValueType.Float && right == ValueType.Float)
        {
          return ValueType.Float;
        }

        if ((left == ValueType.Float && right == ValueType.Int) || (left == ValueType.Int && right == ValueType.Float))
        {
          return ValueType.Float;
        }

        if (left == ValueType.String && right == ValueType.String)
        {
          return ValueType.String;
        }

        return null;
      case BinaryOperation.Minus:
      case BinaryOperation.Multiplication:
      case BinaryOperation.Division:
        if (left == ValueType.Int && right == ValueType.Int)
        {
          return ValueType.Int;
        }

        if (left == ValueType.Float && right == ValueType.Float)
        {
          return ValueType.Float;
        }

        if ((left == ValueType.Float && right == ValueType.Int) || (left == ValueType.Int && right == ValueType.Float))
        {
          return ValueType.Float;
        }

        return null;
      case BinaryOperation.Or:
      case BinaryOperation.And:
      case BinaryOperation.LessThan:
      case BinaryOperation.GreaterThan:
      case BinaryOperation.LessThanOrEqual:
      case BinaryOperation.GreaterThanOrEqual:
        if (left == ValueType.Int && right == ValueType.Int)
        {
          return ValueType.Bool;
        }

        if (left == ValueType.String && right == ValueType.String)
        {
          return ValueType.Bool;
        }

        if (left == ValueType.Float && right == ValueType.Float)
        {
          return ValueType.Bool;
        }

        if (left == ValueType.Bool && right == ValueType.Bool)
        {
          return ValueType.Bool;
        }

        return null;
      case BinaryOperation.Exponentiation:
      case BinaryOperation.Remainder:
        if (left == ValueType.Int && right == ValueType.Int)
        {
          return ValueType.Float;
        }

        if (left == ValueType.Float && right == ValueType.Float)
        {
          return ValueType.Float;
        }

        if ((left == ValueType.Float && right == ValueType.Int) || (left == ValueType.Int && right == ValueType.Float))
        {
          return ValueType.Float;
        }

        return null;
      case BinaryOperation.IntegerDivision:
        if (left == ValueType.Float && right == ValueType.Float)
        {
          return ValueType.Int;
        }

        if ((left == ValueType.Float && right == ValueType.Int) || (left == ValueType.Int && right == ValueType.Float))
        {
          return ValueType.Int;
        }

        return null;
      case BinaryOperation.Equal:
      case BinaryOperation.NotEqual:
        if (left == right && !(left == ValueType.Void && right == ValueType.Void))
        {
          return ValueType.Bool;
        }

        return null;
      default:
        throw new InvalidOperationException($"Unknown binary operation {operation}");
    }
  }
}