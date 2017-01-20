namespace Nop.Plugin.Api.Helpers
{
    public interface IWebConfigMangerHelper
    {
        void AddConfiguration();
        void RemoveConfiguration();

        void AddConnectionString();
    }
}