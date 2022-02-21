namespace MASA.BuildingBlocks.SearchEngine.AutoComplete;
public class AutoCompleteDocument<TValue>
{
    public string Id => IdGenerator();

    public string Text { get; set; }

    public TValue Value { get; set; }

    public AutoCompleteDocument()
    {
    }

    public AutoCompleteDocument(string text, TValue value) : this()
    {
        Text = text;
        Value = value;
    }

    protected virtual string IdGenerator() => $"[{Value}]{Text}";
}
