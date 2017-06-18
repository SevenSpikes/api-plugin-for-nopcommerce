using System;
using System.Xml.Linq;
using System.Xml.XPath;
using Nop.Core;
using Nop.Core.Data;

namespace Nop.Plugin.Api.Helpers
{
    public class WebConfigMangerHelper : IWebConfigMangerHelper
    {
        private readonly IWebHelper _webHelper;

        public WebConfigMangerHelper(IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }

        public void AddConfiguration()
        {
            SetOwinAutomaticAppStartupInWebConfig(true);
        }

        public void AddConnectionString()
        {
            bool hasChanged = false;

            // load web.config
            XDocument webConfig = null;

            using (var fs = System.IO.File.OpenRead(CommonHelper.MapPath("~/Web.config")))
            {
                webConfig = XDocument.Load(fs);
            }

            if (webConfig != null)
            {
                webConfig.Changed += (o, e) => { hasChanged = true; };

                // set versionheader setting
                var connectionStrings = webConfig.XPathSelectElement("configuration//connectionStrings");

                if (connectionStrings == null)
                {
                    var configuration = webConfig.XPathSelectElement("configuration");
                    connectionStrings = new XElement("connectionStrings");
                    configuration.Add(connectionStrings);
                }

                DataSettingsManager dataSettingsManager = new DataSettingsManager();

                DataSettings dataSettings = dataSettingsManager.LoadSettings();

                string connectionStringFromNop = dataSettings.DataConnectionString;

                var element = webConfig.XPathSelectElement("configuration//connectionStrings//add[@name='MS_SqlStoreConnectionString']");

                // create the owin:AutomaticAppStartup attribute if not exists
                if (element == null)
                {                 
                    element = new XElement("add");
                    element.SetAttributeValue("name", "MS_SqlStoreConnectionString");
                    element.SetAttributeValue("connectionString", connectionStringFromNop); 
                    element.SetAttributeValue("providerName", "System.Data.SqlClient");
                    connectionStrings.Add(element);
                }
                else
                {
                    // Check if the connection string is changed.
                    // If so update the connection string in the web.config.
                    string connectionStringInConfig = element.Attribute("connectionString").Value;

                    if (!String.Equals(connectionStringFromNop, connectionStringInConfig, StringComparison.InvariantCultureIgnoreCase))
                    {
                        element.SetAttributeValue("connectionString", connectionStringFromNop);
                    }
                }
                
                if (hasChanged)
                {
                    // only save when changes have been made
                    try
                    {
                        webConfig.Save(CommonHelper.MapPath("~/Web.config"));
                    }
                    catch
                    {
                        // we should do nothing here as throwing an exception breaks nopCommerce.
                        // The right thing to do is to write a message in the Log that the user needs to provide Write access to Web.config
                        // but doing this will lead to many warnings in the Log added after each restart. 
                        // So it is better to do nothing here.
                        //throw new NopException(
                        //    "nopCommerce needs to be restarted due to a configuration change, but was unable to do so." +
                        //    Environment.NewLine +
                        //    "To prevent this issue in the future, a change to the web server configuration is required:" +
                        //    Environment.NewLine +
                        //    "- give the application write access to the 'web.config' file.");
                    }
                }
            }
        }

        public void RemoveConfiguration()
        {
            SetOwinAutomaticAppStartupInWebConfig(false);
        }


        private void SetOwinAutomaticAppStartupInWebConfig(bool value)
        {
            bool hasChanged = false;

            // load web.config
            XDocument webConfig = null;

            using (var fs = System.IO.File.OpenRead(CommonHelper.MapPath("~/Web.config")))
            {
                webConfig = XDocument.Load(fs);
            }

            if (webConfig != null)
            {
                webConfig.Changed += (o, e) => { hasChanged = true; };

                // set versionheader setting
                var httpAppSettings = webConfig.XPathSelectElement("configuration//appSettings");
                var element = webConfig.XPathSelectElement("configuration//appSettings//add[@key='owin:AutomaticAppStartup']");

                // create the owin:AutomaticAppStartup attribute if not exists
                if (element == null)
                {
                    element = new XElement("add");
                    element.SetAttributeValue("key", "owin:AutomaticAppStartup");
                    httpAppSettings.Add(element);
                }
                else
                {
                    // there should be only a single attribute and it should be boolean
                    bool existingValue = Boolean.Parse(element.LastAttribute.Value);

                    if (existingValue != value)
                    {
                        // set the attributes value
                        element.SetAttributeValue("value", value.ToString());
                    }
                }               

                if (hasChanged)
                {
                    // only save when changes have been made
                    try
                    {
                        webConfig.Save(CommonHelper.MapPath("~/Web.config"));
                    }
                    catch
                    {
                        // we should do nothing here as throwing an exception breaks nopCommerce.
                        // The right thing to do is to write a message in the Log that the user needs to provide Write access to Web.config
                        // but doing this will lead to many warnings in the Log added after each restart. 
                        // So it is better to do nothing here.
                        //throw new NopException(
                        //    "nopCommerce needs to be restarted due to a configuration change, but was unable to do so." +
                        //    Environment.NewLine +
                        //    "To prevent this issue in the future, a change to the web server configuration is required:" +
                        //    Environment.NewLine +
                        //    "- give the application write access to the 'web.config' file.");
                    }
                }
            }
        }
    }
}