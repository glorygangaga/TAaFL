using Ast;
using Ast.Declarations;
using Ast.Expressions;

namespace Semantics.Passes;

/// <summary>
/// Базовый класс для проходов по AST с целью вычисления атрибутов и семантических проверок.
/// </summary>
public abstract class AbstractPass : IAstVisitor
{
  public virtual void Visit(LiteralExpression e)
  {
  }

  public virtual void Visit(BinaryOperationExpression e)
  {
    e.Left.Accept(this);
    e.Right.Accept(this);
  }

  public virtual void Visit(SequenceExpression e)
  {
    foreach (Expression nested in e.Sequence)
    {
      nested.Accept(this);
    }
  }

  public virtual void Visit(FunctionCallExpression e)
  {
    foreach (Expression argument in e.Arguments)
    {
      argument.Accept(this);
    }
  }

  public virtual void Visit(AssignmentExpression e)
  {
    e.Left.Accept(this);
    e.Right.Accept(this);
  }

  public virtual void Visit(IfElseExpression e)
  {
    e.Condition.Accept(this);
    e.ThenBranch.Accept(this);
    e.ElseBranch?.Accept(this);
  }

  public virtual void Visit(WhileLoopExpression e)
  {
    e.Condition.Accept(this);
    e.LoopBody.Accept(this);
  }

  public virtual void Visit(ForLoopExpression e)
  {
    e.StartValue.Accept(this);
    e.EndCondition.Accept(this);
    if (e.StepValue != null)
    {
      e.StepValue!.Accept(this);
    }

    e.Body.Accept(this);
  }

  public virtual void Visit(BreakLoopExpression e)
  {
  }

  public virtual void Visit(VariableDeclaration d)
  {
    d.InitialValue!.Accept(this);
  }

  public virtual void Visit(FunctionDeclaration d)
  {
    foreach (ParameterDeclaration declaration in d.Parameters)
    {
      declaration.Accept(this);
    }

    d.Body.Accept(this);
  }

  public virtual void Visit(ParameterDeclaration d)
  {
  }

  public virtual void Visit(UnaryOperationExpression e)
  {
    e.Operand.Accept(this);
  }

  public virtual void Visit(VariableExpression e)
  {
  }

  public virtual void Visit(PrintExpression e)
  {
    foreach (Expression expr in e.Values)
    {
      expr.Accept(this);
    }
  }

  public virtual void Visit(ContinueLoopExpression e)
  {
  }

  public virtual void Visit(ReturnExpression e)
  {
    if (e.Value != null)
    {
      e.Value.Accept(this);
    }
  }

  public virtual void Visit(EmptyExpression e)
  {
  }

  public virtual void Visit(DeclarationExpression e)
  {
    e.Declaration.Accept(this);
  }

  public virtual void Visit(ConstantDeclaration d)
  {
    d.Value.Accept(this);
  }

  public virtual void Visit(BlockStatement s)
  {
    foreach (AstNode nested in s.Statements)
    {
      nested.Accept(this);
    }
  }
}