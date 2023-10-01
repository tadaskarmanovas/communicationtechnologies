namespace Data;

public interface ITextModel
{
    string Text { get; set; }
}

public class TextModel : ITextModel
{
    public string Text { get; set; }
}