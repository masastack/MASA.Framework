namespace MASA.BuildingBlocks.SearchEngine.AutoComplete;
public interface IAutoCompleteFactory
{
    IAutoCompleteClient CreateClient();

    IAutoCompleteClient CreateClient(string name);
}
