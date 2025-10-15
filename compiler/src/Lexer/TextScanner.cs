namespace Lexer;

public class TextScanner(string text)
{
  private readonly string text = text;
  private int position;

  public char Peek(int n = 0)
  {
    int position = this.position + n;
    return text.Length > this.position ? text[position] : '\0';
  }

  public void Advance() => position++;

  public bool IsEnd() => position >= text.Length;
}