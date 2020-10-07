namespace AspnetRunBasics.Settings
{
    public interface IApiSettings
    {
        string BaseAddress { get; set; }

        string CataloguePath { get; set; }

        string BasketPath { get; set; }

        string OrderPath { get; set; }
    }
}
