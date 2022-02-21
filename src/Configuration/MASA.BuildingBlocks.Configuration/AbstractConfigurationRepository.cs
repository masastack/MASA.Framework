namespace MASA.BuildingBlocks.Configuration;
public abstract class AbstractConfigurationRepository : IConfigurationRepository
{
    private readonly ILogger<AbstractConfigurationRepository> _logger;

    private readonly List<IRepositoryChangeListener> _listeners = new();

    public abstract SectionTypes SectionType { get; init; }

    public AbstractConfigurationRepository(ILoggerFactory loggerFactory)
        => _logger = loggerFactory.CreateLogger<AbstractConfigurationRepository>();

    public abstract Properties Load();

    public void AddChangeListener(IRepositoryChangeListener listener)
    {
        if (!_listeners.Contains(listener))
            _listeners.Add(listener);
    }

    public void RemoveChangeListener(IRepositoryChangeListener listener)
        => _listeners.Remove(listener);

    public void FireRepositoryChange(SectionTypes sectionType, Properties newProperties)
    {
        foreach (var listener in _listeners)
        {
            try
            {
                listener.OnRepositoryChange(sectionType, newProperties);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to invoke repository change listener {listener.GetType()}", ex);
            }
        }
    }
}
