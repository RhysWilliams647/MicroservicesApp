namespace Catalogue.API.Settings
{
    public class CatalogueDatabaseSettings : ICatalogueDatabaseSettings
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string CollectionName { get; set; }
    }
}
