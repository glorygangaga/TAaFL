using Xunit;

namespace ExampleLib.UnitTests;

public class TextUtilTest
{
  public static TheoryData<string, int> GetRomanTestData()
  {
    return new TheoryData<string, int>
    {
        { "", 0 },
        { "VII", 7 },
        { "MMXXV", 2025 },
        { "IV", 4 },
        { "IX", 9 },
        { "CM", 900 },
    };
  }

  public static TheoryData<string> GetInvalidRomanData()
  {
    return new TheoryData<string>
    {
        { "Random text" },
        { "IM" },
        { "IIV" },
        { "VX" },
        { "IL" },
        { "IIII" },
        { "MMMI" },
        { "I I I I   " },
    };
  }

  [Fact]
  public void Can_extract_russian_words()
  {
    const string text = """
                            Играют волны — ветер свищет,
                            И мачта гнётся и скрыпит…
                            Увы! он счастия не ищет
                            И не от счастия бежит!
                            """;
    List<string> expected =
    [
        "Играют",
            "волны",
            "ветер",
            "свищет",
            "И",
            "мачта",
            "гнётся",
            "и",
            "скрыпит",
            "Увы",
            "он",
            "счастия",
            "не",
            "ищет",
            "И",
            "не",
            "от",
            "счастия",
            "бежит",
        ];

    List<string> actual = TextUtil.ExtractWords(text);
    Assert.Equal(expected, actual);
  }

  [Fact]
  public void Can_extract_words_with_hyphens()
  {
    const string text = "Что-нибудь да как-нибудь, и +/- что- то ещё";
    List<string> expected =
    [
        "Что-нибудь",
            "да",
            "как-нибудь",
            "и",
            "что",
            "то",
            "ещё",
        ];

    List<string> actual = TextUtil.ExtractWords(text);
    Assert.Equal(expected, actual);
  }

  [Fact]
  public void Can_extract_words_with_apostrophes()
  {
    const string text = "Children's toys and three cats' toys";
    List<string> expected =
    [
        "Children's",
            "toys",
            "and",
            "three",
            "cats'",
            "toys",
        ];

    List<string> actual = TextUtil.ExtractWords(text);
    Assert.Equal(expected, actual);
  }

  [Fact]
  public void Can_extract_words_with_grave_accent()
  {
    const string text = "Children`s toys and three cats` toys, all of''them are green";
    List<string> expected =
    [
        "Children`s",
            "toys",
            "and",
            "three",
            "cats`",
            "toys",
            "all",
            "of'",
            "them",
            "are",
            "green",
        ];

    List<string> actual = TextUtil.ExtractWords(text);
    Assert.Equal(expected, actual);
  }

  [Theory]
  [MemberData(nameof(GetRomanTestData))]
  public void CanParseValidRomanValues(string text, int expected)
  {
    int actual = TextUtil.ParseRoman(text);
    Assert.Equal(expected, actual);
  }

  [Theory]
  [MemberData(nameof(GetInvalidRomanData))]
  public void CanParseInvalidRomanValues(string text)
  {
    Assert.Throws<ArgumentException>(() => TextUtil.ParseRoman(text));
  }
}
