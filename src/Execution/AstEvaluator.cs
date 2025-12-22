using Ast;
using Ast.Declarations;
using Ast.Expressions;

using Execution.Exceptions;

namespace Execution;

public sealed class AstEvaluator : IAstVisitor
{
  private readonly Context context;
  private readonly Stack<decimal> values = new();

  public AstEvaluator(Context context)
  {
    this.context = context;
  }

  public decimal Evaluate(AstNode node)
  {
    if (values.Count > 0)
    {
      throw new InvalidOperationException(
          $"Evaluation stack must be empty, but contains {values.Count} values: {string.Join(", ", values)}"
      );
    }

    node.Accept(this);

    return values.Count switch
    {
      0 => throw new InvalidOperationException(
          "Evaluator logical error: the stack has no evaluation result"
      ),
      > 1 => throw new InvalidOperationException(
          $"Evaluator logical error: expected 1 value, got {values.Count} values: {string.Join(", ", values)}"
      ),
      _ => values.Pop(),
    };
  }

  public void Visit(LiteralExpression e)
  {
    values.Push(e.Value);
  }

  public void Visit(VariableExpression e)
  {
    values.Push(context.GetValue(e.Name));
  }

  public void Visit(AssignmentExpression e)
  {
    e.Value.Accept(this);
    decimal value = values.Pop();
    context.AssignVariable(e.Name, value);
    values.Push(value);
  }

  public void Visit(UnaryOperationExpression e)
  {
    e.Operand.Accept(this);
    decimal v = values.Pop();

    values.Push(e.Operation switch
    {
      UnaryOperation.Plus => +v,
      UnaryOperation.Minus => -v,
      UnaryOperation.Not => v == 0 ? 1 : 0,
      UnaryOperation.Increment => v + 1,
      UnaryOperation.Decrement => v - 1,
      _ => throw new NotImplementedException()
    });
  }

  public void Visit(BinaryOperationExpression e)
  {
    e.Left.Accept(this);
    e.Right.Accept(this);

    decimal right = values.Pop();
    decimal left = values.Pop();

    values.Push(e.Operation switch
    {
      BinaryOperation.Plus => left + right,
      BinaryOperation.Minus => left - right,
      BinaryOperation.Multiplication => left * right,
      BinaryOperation.Division => right == 0 ? throw new DivideByZeroException() : left / right,
      BinaryOperation.IntegerDivision => right == 0 ? throw new DivideByZeroException() : Math.Floor(left / right),
      BinaryOperation.Remainder => right == 0 ? throw new DivideByZeroException() : left % right,
      BinaryOperation.Exponentiation => (decimal)Math.Pow((double)left, (double)right),

      BinaryOperation.And => (left != 0 && right != 0) ? 1 : 0,
      BinaryOperation.Or => (left != 0 || right != 0) ? 1 : 0,

      BinaryOperation.Equal => left == right ? 1 : 0,
      BinaryOperation.NotEqual => left != right ? 1 : 0,
      BinaryOperation.LessThan => left < right ? 1 : 0,
      BinaryOperation.LessThanOrEqual => left <= right ? 1 : 0,
      BinaryOperation.GreaterThan => left > right ? 1 : 0,
      BinaryOperation.GreaterThanOrEqual => left >= right ? 1 : 0,

      _ => throw new NotImplementedException()
    });
  }

  public void Visit(FunctionCallExpression e)
  {
    List<decimal> args = new();

    foreach (Expression arg in e.Arguments)
    {
      arg.Accept(this);
      args.Add(values.Pop());
    }

    if (BuiltinFunctions.Instance.IsBuiltin(e.Name))
    {
      values.Push(BuiltinFunctions.Instance.Invoke(e.Name, args));
      return;
    }

    FunctionDeclaration fn = context.GetFunction(e.Name);

    context.PushScope(new Scope());
    try
    {
      for (int i = 0; i < fn.Parameters.Count; i++)
      {
        context.DefineVariable(fn.Parameters[i], args[i]);
      }

      fn.Body.Accept(this);
      values.Pop();
      values.Push(0);
    }
    catch (ReturnException ret)
    {
      values.Push(ret.Value ?? 0);
    }
    finally
    {
      context.PopScope();
    }
  }

  public void Visit(IfElseExpression e)
  {
    e.Condition.Accept(this);
    decimal cond = values.Pop();

    if (cond != 0)
    {
      e.ThenBranch.Accept(this);
    }
    else if (e.ElseBranch != null)
    {
      e.ElseBranch.Accept(this);
    }
    else
    {
      values.Push(0);
    }
  }

  public void Visit(WhileLoopExpression e)
  {
    context.PushScope(new Scope());
    try
    {
      while (true)
      {
        e.Condition.Accept(this);
        if (values.Pop() == 0)
        {
          break;
        }

        try
        {
          e.LoopBody.Accept(this);
          values.Pop();
        }
        catch (ContinueLoopException)
        {
        }
      }
    }
    catch (BreakLoopException)
    {
    }
    finally
    {
      context.PopScope();
    }

    values.Push(0);
  }

  public void Visit(ForLoopExpression e)
  {
    context.PushScope(new Scope());
    try
    {
      e.StartValue.Accept(this);
      decimal it = values.Pop();
      context.DefineVariable(e.IteratorName, it);

      decimal step = 1;
      if (e.StepValue != null)
      {
        e.StepValue.Accept(this);
        step = values.Pop();
      }

      while (true)
      {
        e.EndCondition.Accept(this);
        if (values.Pop() == 0)
        {
          break;
        }

        try
        {
          e.Body.Accept(this);
          values.Pop();
        }
        catch (ContinueLoopException)
        {
        }

        it += step;
        context.AssignVariable(e.IteratorName, it);
      }
    }
    catch (BreakLoopException)
    {
    }
    finally
    {
      context.PopScope();
    }

    values.Push(0);
  }

  public void Visit(SwitchExpression e)
  {
    e.Expression.Accept(this);
    decimal expr = values.Pop();

    foreach (SwitchCase c in e.Cases)
    {
      c.Value.Accept(this);
      if (values.Pop() == expr)
      {
        try
        {
          c.Body.Accept(this);
          values.Pop();
        }
        catch (BreakLoopException)
        {
        }

        values.Push(0);
        return;
      }
    }

    if (e.DefaultCase != null)
    {
      e.DefaultCase.Accept(this);
      values.Pop();
    }

    values.Push(0);
  }

  public void Visit(SequenceExpression e)
  {
    decimal last = 0;

    foreach (Expression expr in e.Sequence)
    {
      expr.Accept(this);
      last = values.Pop();
    }

    values.Push(last);
  }

  public void Visit(PrintExpression e)
  {
    decimal last = 0;

    foreach (Expression expr in e.Values)
    {
      expr.Accept(this);
      last = values.Pop();
      context.Environment.WriteNumber(last);
    }

    values.Push(last);
  }

  public void Visit(ReturnExpression e)
  {
    decimal value = 0;
    if (e.Value != null)
    {
      e.Value.Accept(this);
      value = values.Pop();
    }

    throw new ReturnException(value);
  }

  public void Visit(BreakLoopExpression e)
  {
    throw new BreakLoopException();
  }

  public void Visit(ContinueLoopExpression e)
  {
    throw new ContinueLoopException();
  }

  public void Visit(EmptyExpression e)
  {
    values.Push(0);
  }

  public void Visit(DeclarationExpression e)
  {
    e.Declaration.Accept(this);
    values.Push(0);
  }

  public void Visit(VariableDeclaration d)
  {
    decimal value = 0;
    if (d.Value != null)
    {
      d.Value.Accept(this);
      value = values.Pop();
    }

    context.DefineVariable(d.Name, value);
  }

  public void Visit(ConstantDeclaration d)
  {
    d.Value.Accept(this);
    context.DefineConstant(d.Name, values.Pop());
  }

  public void Visit(FunctionDeclaration d)
  {
    context.DefineFunction(d);
    values.Push(0);
  }
}
