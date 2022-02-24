namespace Masa.Contrib.Configuration;

public static class MasaConfigurationExtensions
{
    public static void UseMasaOptions(this IMasaConfigurationBuilder builder, Action<MasaRelationOptions> options)
    {
        var relation = new MasaRelationOptions();
        options.Invoke(relation);
        builder.AddRelations(relation.Relations.ToArray());
    }

    internal static void AutoMapping(this MasaConfigurationBuilder builder, params Assembly[] assemblies)
    {
        var optionTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type != typeof(IMasaConfigurationOptions) && type != typeof(MasaConfigurationOptions) && typeof(IMasaConfigurationOptions).IsAssignableFrom(type))
            .ToList();
        optionTypes.ForEach(optionType =>
        {
            var option = (IMasaConfigurationOptions)Activator.CreateInstance(optionType)!;
            var sectionName = option.Section ?? optionType.Name;
            if (builder.Relations.Any(relation => relation.SectionType == option.SectionType && relation.Section == sectionName))
            {
                throw new ArgumentException("The section has been loaded, no need to load repeatedly, check whether there are duplicate sections or inheritance between auto-mapping classes");
            }
            builder.AddRelations(new ConfigurationRelationOptions()
            {
                SectionType = option.SectionType,
                ParentSection = option.ParentSection,
                Section = sectionName,
                ObjectType = optionType
            });
        });
    }
}
