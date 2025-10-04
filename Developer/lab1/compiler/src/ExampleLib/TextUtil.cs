using System.Text;

namespace ExampleLib;

public static class TextUtil
{
  // Символы Unicode, которые мы принимаем как дефис.
  private static readonly Rune[] Hyphens = [new Rune('‐'), new Rune('-')];

  // Символы Unicode, которые мы принимаем как апостроф.
  private static readonly Rune[] Apostrophes = [new Rune('\''), new Rune('`')];

  private static readonly Dictionary<char, int> RomanValues = new()
    {
      { 'I', 1 }, { 'V', 5 }, { 'X', 10 }, { 'L', 50 }, { 'C', 100 }, { 'D', 500 }, { 'M', 1000 },
    };

  private static readonly Dictionary<string, int> DoubleRomanValues = new()
    {
      { "IV", 4 }, { "IX", 9 }, { "XL", 40 }, { "XC", 90 }, { "CD", 400 }, { "CM", 900 },
    };

  // Состояния распознавателя слов.
  private enum WordState
  {
    NoWord,
    Letter,
    Hyphen,
    Apostrophe,
  }

  /// <summary>
  ///  Распознаёт слова в тексте. Поддерживает Unicode, в том числе английский и русский языки.
  ///  Слово состоит из букв, может содержать дефис в середине и апостроф в середине либо в конце.
  /// </summary>
  /// <remarks>
  ///  Функция использует автомат-распознаватель с четырьмя состояниями:
  ///   1. NoWord — автомат находится вне слова;
  ///   2. Letter — автомат находится в буквенной части слова;
  ///   3. Hyphen — автомат обработал дефис;
  ///   4. Apostrophe — автомат обработал апостроф.
  ///
  ///  Переходы между состояниями:
  ///   - NoWord → Letter — при получении буквы;
  ///   - Letter → Hyphen — при получении дефиса;
  ///   - Letter → Apostrophe — при получении апострофа;
  ///   - Letter → NoWord — при получении любого символа, кроме буквы, дефиса или апострофа;
  ///   - Hyphen → Letter — при получении буквы;
  ///   - Hyphen → NoWord — при получении любого символа, кроме буквы;
  ///   - Apostrophe → Letter — при получении буквы;
  ///   - Apostrophe → NoWord — при получении любого символа, кроме буквы.
  ///
  ///  Разница между состояниями Hyphen и Apostrophe в том, что дефис не может стоять в конце слова.
  /// </remarks>
  public static List<string> ExtractWords(string text)
  {
    WordState state = WordState.NoWord;

    List<string> results = [];
    StringBuilder currentWord = new();
    foreach (Rune ch in text.EnumerateRunes())
    {
      switch (state)
      {
        case WordState.NoWord:
          if (Rune.IsLetter(ch))
          {
            PushCurrentWord();
            currentWord.Append(ch);
            state = WordState.Letter;
          }

          break;

        case WordState.Letter:
          if (Rune.IsLetter(ch))
          {
            currentWord.Append(ch);
          }
          else if (Hyphens.Contains(ch))
          {
            currentWord.Append(ch);
            state = WordState.Hyphen;
          }
          else if (Apostrophes.Contains(ch))
          {
            currentWord.Append(ch);
            state = WordState.Apostrophe;
          }
          else
          {
            state = WordState.NoWord;
          }

          break;

        case WordState.Hyphen:
          if (Rune.IsLetter(ch))
          {
            currentWord.Append(ch);
            state = WordState.Letter;
          }
          else
          {
            // Убираем дефис, которого не должно быть в конце слова.
            currentWord.Remove(currentWord.Length - 1, 1);
            state = WordState.NoWord;
          }

          break;

        case WordState.Apostrophe:
          if (Rune.IsLetter(ch))
          {
            currentWord.Append(ch);
            state = WordState.Letter;
          }
          else
          {
            state = WordState.NoWord;
          }

          break;
      }
    }

    PushCurrentWord();

    return results;

    void PushCurrentWord()
    {
      if (currentWord.Length > 0)
      {
        results.Add(currentWord.ToString());
        currentWord.Clear();
      }
    }
  }

  public static int ParseRoman(string text)
  {
    text = text.Trim();

    int total = 0;
    if (string.IsNullOrWhiteSpace(text))
    {
      return total;
    }

    int repeatCount = 1;
    for (int i = 0; i < text.Length; i++)
    {
      if (!RomanValues.TryGetValue(text[i], out int value))
      {
        throw new ArgumentException("Invalid character in Roman numeral.");
      }

      if (i == 0)
      {
        total += value;
        continue;
      }

      string doubleChars = string.Concat(text[i - 1], text[i]);
      if (DoubleRomanValues.TryGetValue(doubleChars, out int doubleValue))
      {
        total += doubleValue - RomanValues[text[i - 1]];
        repeatCount = 1;

        if (i > 1 && RomanValues[text[i - 2]] < doubleValue)
        {
          throw new ArgumentException("The Roman numeral is written incorrectly.");
        }

        continue;
      }

      if (RomanValues[text[i - 1]] < value)
      {
        throw new ArgumentException("The Roman numeral is written incorrectly.");
      }

      if (text[i] == text[i - 1])
      {
        repeatCount++;
        if (repeatCount > 3)
        {
          throw new ArgumentException("The Roman numeral is written incorrectly. More than 3 times.");
        }
      }
      else
      {
        repeatCount = 1;
      }

      total += value;

      if (total > 3000)
      {
        throw new ArgumentException("Total out of range: total > 3000");
      }
    }

    return total;
  }
}
