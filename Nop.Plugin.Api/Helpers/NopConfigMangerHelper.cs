using System.Xml.Linq;
using Nop.Core;
using Nop.Core.Data;

namespace Nop.Plugin.Api.Helpers
{
    using System;
    using System.Reflection;
    using System.Xml.XPath;

    public class NopConfigMangerHelper : IConfigMangerHelper
    {
        public void AddStaticFilesBindingRedirect()
        {
            bool hasChanged = false;

            // load web.config
            XDocument webConfig = null;

            string nopWebAssemblyLoacation = $"{Assembly.GetEntryAssembly().Location}.config";

            using (var fs = System.IO.File.OpenRead(nopWebAssemblyLoacation))
            {
                webConfig = XDocument.Load(fs);
            }

            if (webConfig != null)
            {
                webConfig.Changed += (o, e) => { hasChanged = true; };

                var runtime = webConfig.XPathSelectElement("configuration//runtime");
                
                if (runtime == null)
                {
                    runtime = new XElement("runtime");
                    webConfig.XPathSelectElement("configuration")?.Add(runtime);
                }

                AddAssemblyBinding(runtime, "Microsoft.AspNetCore.StaticFiles", "adb9793829ddae60", "0.0.0.0-2.0.0.0", "2.0.0.0");
                AddAssemblyBinding(runtime, "Microsoft.Extensions.FileProviders.Embedded", "adb9793829ddae60", "0.0.0.0-2.0.0.0", "2.0.0.0");
                AddAssemblyBinding(runtime, "Microsoft.AspNetCore.Mvc.Formatters.Json", "adb9793829ddae60", "0.0.0.0-2.0.0.0", "2.0.0.0");

                if (hasChanged)
                {
                    // only save when changes have been made
                    try
                    {
                        webConfig.Save(nopWebAssemblyLoacation);
                    }
                    catch(Exception ex)
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

        private void AddAssemblyBinding(XElement runtime, string name, string publicToken, string oldVersion, string newVersion)
        {
            XElement element = runtime.XPathSelectElement(
                    $"assemblyBinding//dependentAssembly//assemblyIdentity[@name='{name}']");

            // create the binding redirect if it does not exist
            if (element == null)
            {
                //element.SetAttributeValue("xmlns", "urn:schemas-microsoft-com:asm.v1");

                var dependentAssembly = new XElement("dependentAssembly");

                var assemblyIdentity = new XElement("assemblyIdentity");
                assemblyIdentity.SetAttributeValue("name", name);
                assemblyIdentity.SetAttributeValue("publicKeyToken", publicToken);
                assemblyIdentity.SetAttributeValue("culture", "neutral");

                var bindingRedirect = new XElement("bindingRedirect");
                bindingRedirect.SetAttributeValue("oldVersion", oldVersion);
                bindingRedirect.SetAttributeValue("newVersion", newVersion);

                dependentAssembly.Add(assemblyIdentity);
                dependentAssembly.Add(bindingRedirect);

                XNamespace xNamespace = "urn:schemas-microsoft-com:asm.v1";
                element = new XElement(xNamespace + "assemblyBinding");

                element.Add(dependentAssembly);

                runtime.Add(element);
            }
        }
    }
}