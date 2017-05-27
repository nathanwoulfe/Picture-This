using System;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Xml;
using umbraco.cms.businesslogic.packager;
using Umbraco.Core;

namespace PictureThis
{
    class UmbracoStartup : ApplicationEventHandler
    {
        private const string AppSettingKey = "PictureThisInstalled";
        private const string DashboardPath = "~/config/dashboard.config";

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext context)
        {
            //Check to see if appSetting AnalyticsStartupInstalled is true or even present
            var installAppSetting = WebConfigurationManager.AppSettings[AppSettingKey];

            if (string.IsNullOrEmpty(installAppSetting) || installAppSetting != true.ToString())
            {
                //Add Content dashboard XML
                AddContentSectionDashboard();

                //All done installing our custom stuff
                //As we only want this to run once - not every startup of Umbraco
                var webConfig = WebConfigurationManager.OpenWebConfiguration("/");
                webConfig.AppSettings.Settings.Add(AppSettingKey, true.ToString());
                webConfig.Save();

            }

            //Add OLD Style Package Event
            InstalledPackage.BeforeDelete += InstalledPackage_BeforeDelete;
        }

        /// <summary>
        /// Uninstall Package - Before Delete (Old style events, no V6/V7 equivelant)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InstalledPackage_BeforeDelete(InstalledPackage sender, EventArgs e)
        {
            //Check which package is being uninstalled
            if (sender.Data.Name == "Picture This")
            {
                //Start Uninstall - clean up process...
                RemoveContentSectionDashboard();

                //Remove AppSetting key when all done
                var webConfig = WebConfigurationManager.OpenWebConfiguration("/");
                webConfig.AppSettings.Settings.Remove(AppSettingKey);
                webConfig.Save();
            }
        }

        /// <summary>
        /// Add the dashboard tab
        /// </summary>
        private void AddContentSectionDashboard()
        {
            bool saveFile = false;

            //Path to the file resolved
            var dashboardFilePath = HostingEnvironment.MapPath(DashboardPath);

            //Load settings.config XML file
            XmlDocument dashboardXml = new XmlDocument();
            dashboardXml.Load(dashboardFilePath);

            XmlNode firstTab = dashboardXml.SelectSingleNode("//section [@alias='StartupSettingsDashboardSection']/areas");

            if (firstTab != null)
            {
                var xmlToAdd = "<tab caption='Login image'>" +
                                    "<control addPanel='true' panelCaption=''>/app_plugins/picturethis/view.html</control>" +
                                "</tab>";

                //Load in the XML string above
                XmlDocumentFragment frag = dashboardXml.CreateDocumentFragment();
                frag.InnerXml = xmlToAdd;

                //Append the xml above to the dashboard node
                dashboardXml.SelectSingleNode("//section [@alias='StartupSettingsDashboardSection']").AppendChild(frag);

                //Save the file flag to true
                saveFile = true;
            }

            //If saveFile flag is true then save the file
            if (saveFile)
            {
                //Save the XML file
                dashboardXml.Save(dashboardFilePath);
            }
        }

        /// <summary>
        /// Remove the dashboard tab
        /// </summary>
        private void RemoveContentSectionDashboard()
        {
            bool saveFile = false;

            //Path to the file resolved
            var dashboardFilePath = HostingEnvironment.MapPath(DashboardPath);

            //Load settings.config XML file
            XmlDocument dashboardXml = new XmlDocument();
            dashboardXml.Load(dashboardFilePath);

            XmlNode areas = dashboardXml.SelectSingleNode("//section [@alias='StartupSettingsDashboardSection']/areas");

            if (areas != null)
            {
                XmlNode tabToRemove = areas.SelectSingleNode("./tab [@caption='Picture This']");
                if (tabToRemove != null)
                {
                    areas.RemoveChild(tabToRemove);
                    saveFile = true;
                }                
            }

            //If saveFile flag is true then save the file
            if (saveFile)
            {
                //Save the XML file
                dashboardXml.Save(dashboardFilePath);
            }
        }
    }    
}
