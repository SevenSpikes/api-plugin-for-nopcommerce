namespace Nop.Plugin.Api.Helpers
{
    public interface IWebConfigMangerHelper
    {
        void AddConfiguration();
        void RemoveConfiguration();

        // In order for us to store the Web Hooks into the database, the database connection string
        // should be present in the web.config file. And the connection string should have a specific name.
        // This is required by the Microsoft.AspNet.WebHooks library implementation.
        void AddConnectionString();
    }
}