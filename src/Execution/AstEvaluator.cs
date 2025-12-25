using Ast;
using Ast.Declarations;
using Ast.Expressions;

using Execution.Exceptions;

using Runtime;

using ValueType = Runtime.ValueType;

namespace Execution;

public sealed class AstEvaluator : IAstVisitor
{
  private readonly Context context;
  private readonly Stack<Value> values = new();

  public AstEvaluator(Context context)
  {
    this.context = context;
  }

  public Value Evaluate(AstNode node)
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
    e.Right.Accept(this);
    Value value = values.Pop();

    VariableExpression left = (VariableExpression)e.Left;

    context.AssignVariable(left.Name, value);
    values.Push(value);
  }

  public void Visit(UnaryOperationExpression e)
  {
    e.Operand.Accept(this);

    Value operand = values.Pop();

    switch (e.Operation)
    {
      case UnaryOperation.Plus:
        {
          if (operand.IsInt())
          {
            values.Push(new Value(+operand.AsInt()));
            return;
          }

          if (operand.IsFloat())
          {
            values.Push(new Value(+operand.AsFloat()));
            return;
          }

          throw new InvalidOperationException($"Unary '+' is not applicable to {operand}");
        }

      case UnaryOperation.Minus:
        {
          if (operand.IsInt())
          {
            values.Push(new Value(-operand.AsInt()));
            return;
          }

          if (operand.IsFloat())
          {
            values.Push(new Value(-operand.AsFloat()));
            return;
          }

          throw new InvalidOperationException($"Unary '-' is not applicable to {operand}");
        }

      case UnaryOperation.Not:
        {
          bool result = operand switch
          {
            { } when operand.IsBool() => !operand.AsBool(),
            { } when operand.IsInt() => operand.AsInt() == 0,
            { } when operand.IsFloat() => Math.Abs(operand.AsFloat()) < 0.0001f,
            { } when operand.IsString() => string.IsNullOrEmpty(operand.AsString()),
            _ => throw new InvalidOperationException($"Unary 'not' is not applicable to {operand}")
          };

          values.Push(new Value(result));
          return;
        }

      case UnaryOperation.Increment:
        {
          if (operand.IsInt())
          {
            values.Push(new Value(operand.AsInt() + 1));
            return;
          }

          if (operand.IsFloat())
          {
            values.Push(new Value(operand.AsFloat() + 1));
            return;
          }

          throw new InvalidOperationException($"Unary '++' is not applicable to {operand}");
        }

      case UnaryOperation.Decrement:
        {
          if (operand.IsInt())
          {
            values.Push(new Value(operand.AsInt() - 1));
            return;
          }

          if (operand.IsFloat())
          {
            values.Push(new Value(operand.AsFloat() - 1));
            return;
          }

          throw new InvalidOperationException($"Unary '--' is not applicable to {operand}");
        }

      default:
        throw new NotImplementedException($"Unknown unary operation {e.Operation}");
    }
  }

  public void Visit(BinaryOperationExpression e)
  {
    values.Push(EvaluationUtil.ApplyBinaryOperation(e.Operation, EvaluateLeft, EvaluateRight));

    Value EvaluateLeft()
    {
      e.Left.Accept(this);
      return values.Pop();
    }

    Value EvaluateRight()
    {
      e.Right.Accept(this);
      return values.Pop();
    }
  }

  public void Visit(FunctionCallExpression e)
  {
    List<Value> args = new();
    bool hasReturn = false;

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

    bool hasReturnType = fn.DeclaredTypeName != null;

    context.PushScope(new Scope());
    try
    {
      if (args.Count != fn.Parameters.Count)
      {
        throw new ArgumentException(
          $"Function '{e.Name}' expects {fn.Parameters.Count} arguments, but got {args.Count}");
      }

      for (int i = 0; i < fn.Parameters.Count; i++)
      {
        context.DefineVariable(fn.Parameters[i].Name, args[i]);
      }

      fn.Body.Accept(this);
      values.Pop();
      values.Push(Value.Void);

      if (!hasReturn && hasReturnType)
      {
        throw new InvalidOperationException("Function has to have a return statement in the end");
      }
    }
    catch (ReturnException ret)
    {
      hasReturn = true;

      if (ret.Value == null)
      {
        values.Push(Value.Void);
        return;
      }

      ValueType getRetType = GetTypeByValue(ret.Value);

      values.Push(ret.Value);
    }
    finally
    {
      context.PopScope();
    }
  }

  public void Visit(IfElseExpression e)
  {
    e.Condition.Accept(this);

    if (values.Pop().AsBool())
    {
      e.ThenBranch.Accept(this);
    }
    else if (e.ElseBranch != null)
    {
      e.ElseBranch.Accept(this);
    }
    else
    {
      values.Push(Value.Void);
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
        if (!values.Pop().AsBool())
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

    values.Push(Value.Void);
  }

  public void Visit(ForLoopExpression e)
  {
    context.PushScope(new Scope());
    try
    {
      e.StartValue.Accept(this);
      int it = values.Pop().AsInt();
      context.DefineVariable(e.IteratorName, new Value(it));

      int step = 1;
      if (e.StepValue != null)
      {
        e.StepValue.Accept(this);
        step = values.Pop().AsInt();
      }

      while (true)
      {
        e.EndCondition.Accept(this);
        if (!values.Pop().AsBool())
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
        context.AssignVariable(e.IteratorName, new Value(it));
      }
    }
    catch (BreakLoopException)
    {
    }
    finally
    {
      context.PopScope();
    }

    values.Push(Value.Void);
  }

  public void Visit(SequenceExpression e)
  {
    Value last = new Value(0);

    foreach (Expression expr in e.Sequence)
    {
      expr.Accept(this);
      last = values.Pop();
    }

    values.Push(last);
  }

  public void Visit(PrintExpression e)
  {
    Value last = new Value(0);

    foreach (Expression expr in e.Values)
    {
      expr.Accept(this);
      last = values.Pop();
      context.Environment.Write(last);
    }

    values.Push(last);
  }

  public void Visit(ReturnExpression e)
  {
    Value? value = null;
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
    values.Push(Value.Void);
  }

  public void Visit(DeclarationExpression e)
  {
    e.Declaration.Accept(this);
    values.Push(Value.Void);
  }

  public void Visit(VariableDeclaration d)
  {
    Value value = Value.Void;
    if (d.InitialValue != null)
    {
      d.InitialValue.Accept(this);
      value = values.Pop();
    }

    context.DefineVariable(d.Name, value);
  }

  public void Visit(ConstantDeclaration d)
  {
    d.Value.Accept(this);
    Value val = values.Pop();
    context.DefineConstant(d.Name, val);
  }

  public void Visit(FunctionDeclaration d)
  {
    context.DefineFunction(d);
    values.Push(Value.Void);
  }

  public void Visit(ParameterDeclaration d)
  {
  }

  public void Visit(BlockStatement s)
  {
    values.Push(Value.Void);
    context.PushScope(new Scope());

    try
    {
      foreach (AstNode node in s.Statements)
      {
        values.Pop();
        node.Accept(this);
      }
    }
    finally
    {
      context.PopScope();
    }
  }

  private ValueType GetTypeByValue(Value value)
  {
    return value switch
    {
      { } when value.IsBool() => ValueType.Bool,
      { } when value.IsFloat() => ValueType.Float,
      { } when value.IsString() => ValueType.String,
      { } when value.IsInt() => ValueType.Int,
      { } when value == Value.Void => ValueType.Void,
      _ => throw new NotImplementedException()
    };
  }
}
