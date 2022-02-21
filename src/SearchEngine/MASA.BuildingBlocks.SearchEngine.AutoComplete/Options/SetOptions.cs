namespace MASA.BuildingBlocks.SearchEngine.AutoComplete.Options;
public class SetOptions
{
    public bool IsOverride { get; set; }

    public SetOptions(bool isOverride = true)
    {
        IsOverride = isOverride;
    }
}
