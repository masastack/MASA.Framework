namespace MASA.BuildingBlocks.Configuration;
public interface IRepositoryChangeListener
{
    void OnRepositoryChange(SectionTypes sectionType, Properties newProperties);
}
