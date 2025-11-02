using Lexer;

namespace Parser;

#pragma warning disable RCS1194 // Implement exception constructors
public class UnexpectedLexemeException : Exception
#pragma warning restore RCS1194 // Implement exception constructors
{
  public UnexpectedLexemeException(TokenType expected, Token actual)
        : base($"Unexpected lexeme {actual} where expected {expected}")
  {
  }
}