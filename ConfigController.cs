using System.Web.Http;
using System.Web.Hosting;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using System.Xml;
using System;

namespace PictureThis.Api
{
    [PluginController("PictureThis")]
    public class ConfigController : UmbracoAuthorizedApiController
    {
        const string configPath = "~/config/umbracoSettings.config";
        private string configFilePath;
        private XmlDocument xmlDoc = new XmlDocument();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get()
        {
            configFilePath = HostingEnvironment.MapPath(configPath);
            xmlDoc.Load(configFilePath);

            return Ok(new { image = xmlDoc.SelectSingleNode("//loginBackgroundImage").InnerText });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Post(ImageModel model)
        {
            try
            {
                bool saveFile = false;

                configFilePath = HostingEnvironment.MapPath(configPath);
                xmlDoc.Load(configFilePath);
                XmlNode imageNode = xmlDoc.SelectSingleNode("//loginBackgroundImage");

                if (imageNode != null)
                {
                    imageNode.InnerText = model.Image;
                    saveFile = true;
                }

                if (saveFile)
                {
                    xmlDoc.Save(configFilePath);
                    return Ok(new { status = 200, message = "Background image updated" });
                }

                return Content(System.Net.HttpStatusCode.InternalServerError, "Background image not updated");
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public class ImageModel
        {
            public string Image { get; set; }
        }
    }
}
