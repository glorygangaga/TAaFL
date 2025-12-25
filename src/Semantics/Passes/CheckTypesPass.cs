using Ast.Declarations;
using Ast.Expressions;

using Semantics.Exceptions;
using Semantics.Helpers;

using ValueType = Runtime.ValueType;

namespace Semantics.Passes;

/// <summary>
/// Проход по AST для проверки корректности программы с точки зрения совместимости типов данных.
/// </summary>
/// <exception cref="TypeErrorException">Бросается при несоответствии типов данных в процессе проверки.</exception>
public class CheckTypesPass : AbstractPass
{
  /// <summary>
  /// Проверяет соответствие типов параметров функции и аргументов при вызове этой функции.
  /// </summary>
  public override void Visit(FunctionCallExpression e)
  {
    base.Visit(e);
    CheckFunctionArgumentTypes(e, e.Function);
  }

  public override void Visit(FunctionDeclaration d)
  {
    base.Visit(d);
    CheckAreSameTypes("function body", d.Body, d.ResultType);
  }

  public override void Visit(VariableDeclaration d)
  {
    base.Visit(d);

    ValueType inferredType = d.InitialValue!.ResultType;

    if (d.DeclaredType != null && d.DeclaredType.ResultType != inferredType && inferredType != ValueType.Void)
    {
      throw new TypeErrorException(
          $"Cannot initialize variable of type {d.DeclaredTypeName} with value of type {inferredType}"
      );
    }

    if (d.DeclaredType == null && inferredType == ValueType.Void)
    {
      throw new TypeErrorException(
          $"Variable {d.Name} type cannot be inferred from nil"
      );
    }
  }

  public override void Visit(ConstantDeclaration d)
  {
    base.Visit(d);

    ValueType inferredType = d.Value.ResultType;

    if (inferredType == ValueType.Void)
    {
      throw new TypeErrorException("Cannot initialize Const from expression without value");
    }

    if (d.DeclaredType != null && d.DeclaredType.ResultType != inferredType && inferredType != ValueType.Void)
    {
      throw new TypeErrorException(
          $"Cannot initialize Const of type {d.DeclaredTypeName} with value of type {inferredType}"
      );
    }

    if (d.DeclaredType == null && inferredType == ValueType.Void)
    {
      throw new TypeErrorException(
          $"Variable {d.Name} type cannot be inferred from nil"
      );
    }
  }

  public override void Visit(AssignmentExpression e)
  {
    base.Visit(e);

    if (e.Left.ResultType != e.Right.ResultType)
    {
      throw new TypeErrorException(
          $"Cannot assign value of type {e.Right.ResultType} to variable of type {e.Left.ResultType}"
      );
    }
  }

  public override void Visit(IfElseExpression e)
  {
    base.Visit(e);

    CheckAreSameTypes("if-else condition", e.Condition, ValueType.Int);

    ValueType thenType = e.ThenBranch.ResultType;
    if (e.ElseBranch != null)
    {
      CheckAreCompatibleTypes("else branch", e.ElseBranch, thenType);
    }
    else if (thenType != ValueType.Void)
    {
      throw new TypeErrorException("The \"if...then\" expression without \"else\" branch may not return value");
    }
  }

  public override void Visit(WhileLoopExpression e)
  {
    base.Visit(e);

    CheckAreSameTypes("while loop condition", e.Condition, ValueType.Int);
    CheckAreSameTypes("while loop body", e.LoopBody, ValueType.Void);
  }

  public override void Visit(ForLoopExpression e)
  {
    base.Visit(e);

    CheckAreSameTypes("for loop end value", e.EndCondition, ValueType.Int);
    CheckAreSameTypes("for loop body", e.Body, ValueType.Void);
  }

  /// <summary>
  /// Проверяет соответствие типов формальных параметров и фактических параметров (аргументов) при вызове функции.
  /// </summary>
  private static void CheckFunctionArgumentTypes(FunctionCallExpression e, AbstractFunctionDeclaration function)
  {
    // Для каждого i-го аргумента выводим тип и сверяем с типом i-го параметра функции.
    for (int i = 0, iMax = e.Arguments.Count; i < iMax; ++i)
    {
      Expression argument = e.Arguments[i];
      AbstractParameterDeclaration parameter = function.Parameters[i];
      if (argument.ResultType != parameter.ResultType)
      {
        throw new TypeErrorException(
            $"Cannot apply argument #{i} of type {argument.ResultType} to function {e.Name} parameter {parameter.Name} which has type {parameter.ResultType}"
        );
      }
    }
  }

  private static void CheckAreSameTypes(string category, Expression expression, ValueType expectedType)
  {
    if (expression.ResultType != expectedType)
    {
      throw new TypeErrorException(category, expectedType, expression.ResultType);
    }
  }

  private static void CheckAreCompatibleTypes(string category, Expression expression, ValueType expectedType)
  {
    if (expression.ResultType != expectedType)
    {
      throw new TypeErrorException(category, expectedType, expression.ResultType);
    }
  }
}