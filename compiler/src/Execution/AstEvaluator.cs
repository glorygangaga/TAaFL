using Ast;
using Ast.Declarations;
using Ast.Expressions;

namespace Execution;

public class AstEvaluator : IAstVisitor
{
  private readonly Context context;

  private readonly Stack<decimal> values = [];

  public AstEvaluator(Context newContext)
  {
    context = newContext;
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

  public void Visit(AssignmentExpression e)
  {
    e.Value.Accept(this);
    decimal value = values.Peek();
    context.AssignVariable(e.Name, value);
  }

  public void Visit(BinaryOperationExpression e)
  {
    e.Left.Accept(this);
    e.Right.Accept(this);
    decimal right = values.Pop();
    decimal left = values.Pop();

    switch (e.Operation)
    {
      case BinaryOperation.Plus:
        values.Push(left + right);
        break;
      case BinaryOperation.Minus:
        values.Push(left - right);
        break;
      case BinaryOperation.Multiplication:
        values.Push(left * right);
        break;
      case BinaryOperation.Division:
        if (right == 0)
        {
          throw new DivideByZeroException();
        }

        values.Push(left / right);
        break;
      case BinaryOperation.Exponentiation:
        values.Push((decimal)Math.Pow((double)left, (double)right));
        break;
      case BinaryOperation.Remainder:
        if (right == 0)
        {
          throw new DivideByZeroException();
        }

        values.Push(left % right);
        break;
      case BinaryOperation.IntegerDivision:
        if (right == 0)
        {
          throw new DivideByZeroException();
        }

        values.Push(Math.Floor(left / right));
        break;
      case BinaryOperation.And:
        values.Push((left != 0 && right != 0) ? 1 : 0);
        break;
      case BinaryOperation.Or:
        values.Push((left != 0 || right != 0) ? 1 : 0);
        break;
      case BinaryOperation.Equal:
        values.Push(left == right ? 1 : 0);
        break;
      case BinaryOperation.NotEqual:
        values.Push(left != right ? 1 : 0);
        break;
      case BinaryOperation.LessThan:
        values.Push(left < right ? 1 : 0);
        break;
      case BinaryOperation.LessThanOrEqual:
        values.Push(left <= right ? 1 : 0);
        break;
      case BinaryOperation.GreaterThan:
        values.Push(left > right ? 1 : 0);
        break;
      case BinaryOperation.GreaterThanOrEqual:
        values.Push(left >= right ? 1 : 0);
        break;
      default:
        throw new NotImplementedException($"Unknown binary operation {e.Operation}");
    }
  }

  public void Visit(ForLoopExpression e)
  {
    context.PushScope(new Scope());
    try
    {
      e.StartValue.Accept(this);
      decimal iteratorValue = values.Pop();

      decimal stepValue = 1;
      if (e.StepValue != null)
      {
        e.StepValue.Accept(this);
        stepValue = values.Pop();
      }

      context.DefineVariable(e.IteratorName, iteratorValue);

      while (true)
      {
        e.EndCondition.Accept(this);
        decimal endCondition = values.Pop();

        if (endCondition == 0)
        {
          break;
        }

        e.Body.Accept(this);
        values.Pop();

        iteratorValue += stepValue;
        context.AssignVariable(e.IteratorName, iteratorValue);
      }

      values.Push(0);
    }
    finally
    {
      context.PopScope();
    }
  }

  public void Visit(FunctionCallExpression e)
  {
    List<decimal> argValues = new List<decimal>();
    foreach (Expression argument in e.Arguments)
    {
      argument.Accept(this);
      argValues.Add(values.Pop());
    }

    if (BuiltinFunctions.Instance.IsBuiltin(e.Name))
    {
      decimal result = BuiltinFunctions.Instance.Invoke(e.Name, argValues);
      values.Push(result);
      return;
    }

    FunctionDeclaration function = context.GetFunction(e.Name);
    context.PushScope(new Scope());

    try
    {
      foreach (string name in Enumerable.Reverse(function.Parameters))
      {
        context.DefineVariable(name, values.Pop());
      }

      function.Body.Accept(this);
    }
    finally
    {
      context.PopScope();
    }
  }

  public void Visit(IfElseExpression e)
  {
    e.Condition.Accept(this);

    decimal conditionValue = values.Pop();
    bool isTrueCondition = conditionValue != 0;

    if (isTrueCondition)
    {
      e.ThenBranch.Accept(this);
    }
    else
    {
      e.ElseBranch.Accept(this);
    }
  }

  public void Visit(LiteralExpression e)
  {
    values.Push(e.Value);
  }

  public void Visit(SequenceExpression e)
  {
    decimal last = 0;

    foreach (Expression nested in e.Sequence)
    {
      nested.Accept(this);
      last = values.Pop();
    }

    values.Push(last);
  }

  public void Visit(UnaryOperationExpression e)
  {
    e.Operand.Accept(this);
    switch (e.Operation)
    {
      case UnaryOperation.Plus:
        values.Push(+values.Pop());
        break;
      case UnaryOperation.Minus:
        values.Push(-values.Pop());
        break;
      case UnaryOperation.Not:
        values.Push(values.Pop() == 0 ? 1 : 0);
        break;
      case UnaryOperation.Increment:
        values.Push(values.Pop() + 1);
        break;
      case UnaryOperation.Decrement:
        values.Push(values.Pop() - 1);
        break;
      default:
        throw new NotImplementedException($"Unknown unary operation {e.Operation}");
    }
  }

  public void Visit(VariableExpression e)
  {
    values.Push(context.GetValue(e.Name));
  }

  public void Visit(VariableScopeExpression e)
  {
    context.PushScope(new Scope());
    try
    {
      foreach (VariableDeclaration variable in e.Variables)
      {
        variable.Accept(this);
        values.Pop();
      }

      e.Expression.Accept(this);
    }
    finally
    {
      context.PopScope();
    }
  }

  public void Visit(PrintExpression e)
  {
    e.Value.Accept(this);

    decimal value = values.Pop();

    context.Environment.WriteNumber(value);
    values.Push(value);
  }

  public void Visit(ConstantDeclaration d)
  {
    d.Value.Accept(this);
    decimal value = values.Peek();
    context.DefineConstant(d.Name, value);
  }

  public void Visit(FunctionDeclaration d)
  {
    context.DefineFunction(d);
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
    values.Push(value);
  }
}