namespace MASA.BuildingBlocks.SearchEngine.AutoComplete.Response;

public class SetResponse : ResponseBase
{
    public List<SetResponseItems> Items { get; set; }

    public SetResponse(bool isValid, string message) : base(isValid, message)
    {
    }
}
