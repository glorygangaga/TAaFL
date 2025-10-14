namespace Lexer;

public class TextScanner(string text)
{
  private readonly string text = text;
  private int position;

  public char Peek(int n = 0)
  {
    int position = n;
    return text.Length > position ? text[position] : '\0';
  }

  public void Advance()
  {
    position++;
  }

  public bool IsEnd()
  {
    return position >= text.Length;
  }
}