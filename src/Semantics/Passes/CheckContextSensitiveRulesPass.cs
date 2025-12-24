using Ast.Declarations;
using Ast.Expressions;

using Semantics.Exceptions;

namespace Semantics.Passes;

/// <summary>
/// Проверяет соблюдение контекстно-зависимых правил языка.
/// </summary>
/// <remarks>
/// Контекстно-зависимые правила не могли быть проверены при синтаксическом анализе, поскольку синтаксический анализатор
///  разбирает контекстно-свободную грамматику.
/// </remarks>
public sealed class CheckContextSensitiveRulesPass : AbstractPass
{
  private readonly Stack<ExpressionContext> expressionContextStack;

  public CheckContextSensitiveRulesPass()
  {
    expressionContextStack = [];
    expressionContextStack.Push(ExpressionContext.Default);
  }

  private enum ExpressionContext
  {
    Default,
    InsideLoop,
    InsideFunction,
  }

  /// <summary>
  /// Проверяет корректность программы с точки зрения использования функций.
  /// </summary>
  /// <exception cref="InvalidFunctionCallException">Бросается при неправильном вызове функций.</exception>
  public override void Visit(FunctionCallExpression e)
  {
    base.Visit(e);

    if (e.Arguments.Count != e.Function.Parameters.Count)
    {
      throw new InvalidFunctionCallException(
          $"Function {e.Name} requires {e.Function.Parameters.Count} arguments, got {e.Arguments.Count}"
      );
    }
  }

  public override void Visit(FunctionDeclaration d)
  {
    expressionContextStack.Push(ExpressionContext.InsideFunction);
    try
    {
      base.Visit(d);
    }
    finally
    {
      expressionContextStack.Pop();
    }
  }

  public override void Visit(WhileLoopExpression e)
  {
    // Меняем текущий контекст: дочерние узлы AST находятся внутри цикла.
    expressionContextStack.Push(ExpressionContext.InsideLoop);
    try
    {
      base.Visit(e);
    }
    finally
    {
      expressionContextStack.Pop();
    }
  }

  public override void Visit(ForLoopExpression e)
  {
    // Меняем текущий контекст: дочерние узлы AST находятся внутри цикла.
    expressionContextStack.Push(ExpressionContext.InsideLoop);
    try
    {
      base.Visit(e);
    }
    finally
    {
      expressionContextStack.Pop();
    }
  }

  public override void Visit(SwitchExpression e)
  {
    expressionContextStack.Push(ExpressionContext.InsideLoop);
    try
    {
      base.Visit(e);
    }
    finally
    {
      expressionContextStack.Pop();
    }
  }

  public override void Visit(BreakLoopExpression e)
  {
    base.Visit(e);

    // Контекстно-зависимое правило: "break" допускается только внутри цикла,
    //  расположенного в пределах текущей функции.
    if (expressionContextStack.Peek() != ExpressionContext.InsideLoop)
    {
      throw new InvalidExpressionException("The \"break\" expression is allowed only inside the loop");
    }
  }

  public override void Visit(ContinueLoopExpression e)
  {
    base.Visit(e);
    if (expressionContextStack.Peek() != ExpressionContext.InsideLoop)
    {
      throw new InvalidExpressionException("The \"continue\" expression is allowed only inside the loop");
    }
  }

  public override void Visit(ReturnExpression e)
  {
    base.Visit(e);
    if (expressionContextStack.Peek() != ExpressionContext.InsideFunction)
    {
      List<ExpressionContext> contextList = expressionContextStack.ToList();
      ExpressionContext outerContext = ExpressionContext.Default;

      foreach (ExpressionContext context in contextList)
      {
        if (context is not ExpressionContext.InsideLoop)
        {
          outerContext = context;
          break;
        }
      }

      if (outerContext is not ExpressionContext.InsideFunction)
      {
        throw new InvalidExpressionException("The \"return\" expression is allowed only inside the function");
      }
    }
  }
}