namespace MASA.BuildingBlocks.SearchEngine.AutoComplete.Options;
public class AutoCompleteOptions
{
    public string Field { get; set; }

    private int _page;

    public int Page
    {
        get => _page;
        set
        {
            if (value <= 0)
                throw new ArgumentException($"{nameof(Page)} must be greater than 0", nameof(Page));

            _page = value;
        }
    }

    private int _pageSize;

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value <= 0)
                throw new ArgumentException($"{nameof(PageSize)} must be greater than 0", nameof(PageSize));

            _pageSize = value;
        }
    }

    public SearchType SearchType { get; }

    public AutoCompleteOptions(SearchType searchType = SearchType.Fuzzy)
    {
        this.Field = "id";
        this.Page = 1;
        this.PageSize = 10;
        this.SearchType = searchType;
    }
}
