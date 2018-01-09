using System.Data.Entity;
using Nop.Core.Infrastructure;
using Nop.Core.Data;
using Microsoft.AspNet.WebHooks.Diagnostics;
using Nop.Plugin.Api.Services;

namespace Nop.Plugin.Api.WebHooks
{
    public class WebHooksStartupTask : IStartupTask
    {
        public void Execute()
        {
            // We need to add a binding redirect and the connection string in the Nop.Web.config

            //<assemblyIdentity name="Microsoft.AspNetCore.DataProtection.Abstractions" publicKeyToken="adb9793829ddae60" culture="neutral" />
            //   < bindingRedirect oldVersion = "0.0.0.0-2.0.0.0" newVersion = "2.0.0.0" />

            // <connectionStrings>
            // < add name = "MS_SqlStoreConnectionString" connectionString = "Data Source=POYKER\SQL2016;Initial Catalog=nop4_api;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=xxx" providerName = "System.Data.SqlClient" />
            //   </ connectionStrings >
            
            //System.Data.Entity.Database.SetInitializer<Microsoft.AspNet.WebHooks.WebHookStoreContext>(null);

            var dataSettings = EngineContext.Current.Resolve<DataSettings>();
            Microsoft.AspNet.WebHooks.Config.SettingsDictionary settings = new Microsoft.AspNet.WebHooks.Config.SettingsDictionary();
            settings.Add("MS_SqlStoreConnectionString", dataSettings.DataConnectionString);
            settings.Connections.Add("MS_SqlStoreConnectionString", new Microsoft.AspNet.WebHooks.Config.ConnectionSettings("MS_SqlStoreConnectionString", dataSettings.DataConnectionString));

            ILogger logger = new NopWebHooksLogger();
            Microsoft.AspNet.WebHooks.IWebHookStore store = new Microsoft.AspNet.WebHooks.SqlWebHookStore(settings, logger);

            Microsoft.AspNet.WebHooks.Services.CustomServices.SetStore(store);

            // This is required only in development.
            // It it is required only when you want to send a web hook to an https address with an invalid SSL certificate. (self-signed)
            // The code marks all certificates as valid.
            // We may want to extract this as a setting in the future.

            // NOTE: If this code is commented the certificates will be validated.
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        public int Order
        {
            //ensure that this task is run first 
            get { return 0; }
        }
    }
}
