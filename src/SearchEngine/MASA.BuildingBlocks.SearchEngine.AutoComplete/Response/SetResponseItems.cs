namespace MASA.BuildingBlocks.SearchEngine.AutoComplete.Response;
public class SetResponseItems
{
    public string Id { get; }

    public bool IsValid { get; }

    public string Message { get; }

    public SetResponseItems(string id, bool isValid, string message)
    {
        Id = id;
        IsValid = isValid;
        Message = message;
    }
}
