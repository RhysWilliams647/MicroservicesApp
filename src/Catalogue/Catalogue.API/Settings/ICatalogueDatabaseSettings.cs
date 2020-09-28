namespace Catalogue.API.Settings
{
    public interface ICatalogueDatabaseSettings
    {
        string ConnectionString { get; set; }

        string DatabaseName { get; set; }

        string CollectionName { get; set; }
    }
}
