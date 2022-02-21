namespace MASA.BuildingBlocks.Configuration;
public interface IConfigurationRepository
{
    SectionTypes SectionType { get; init; }

    Properties Load();

    void AddChangeListener(IRepositoryChangeListener listener);

    void RemoveChangeListener(IRepositoryChangeListener listener);
}
