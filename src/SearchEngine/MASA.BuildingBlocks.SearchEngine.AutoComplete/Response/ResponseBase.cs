namespace MASA.BuildingBlocks.SearchEngine.AutoComplete.Response;
public class ResponseBase
{
    public bool IsValid { get; }

    public string Message { get; }

    public ResponseBase(bool isValid, string message)
    {
        IsValid = isValid;
        Message = message;
    }
}
