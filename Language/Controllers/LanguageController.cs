using Language.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Language.Controllers
{
    public class LanguageController : Controller
    {
        // GET: Language
        public static string selectLG = "en";

        public ActionResult Index(string ddlCulture)
        {
            if (ddlCulture != null)
            {
                selectLG = ddlCulture;
            }
            
            //return View(this.listLG);
            return View();
        }
        public void x(string ddlCulture)
        {
            selectLG = ddlCulture;
        }
        //public List<LanguageModel> selectList()
        //{
            
        //    XmlDocument doc = new XmlDocument();

        //    if (selectLG == "en"|| selectLG== "" )
        //    {
        //        doc.Load(string.Concat(Server.MapPath(@"~/Assets/Client/data/Account.en.xml")));

        //        foreach (XmlNode node in doc.SelectNodes("/LocalLanguages/data "))
        //        {
        //            this.listLG.Add(new LanguageModel
        //            {
        //                key = node.Attributes["name"].Value,
        //                value = node["value"].InnerText
        //            }); ;
        //        }
        //    }
        //    else
        //    {
        //        doc.Load(string.Concat(Server.MapPath(@"~/Assets/Client/data/Account.km.xml")));

        //        foreach (XmlNode node in doc.SelectNodes("/LocalLanguages/data "))
        //        {
        //            this.listLG.Add(new LanguageModel
        //            {
        //                key = node.Attributes["name"].Value,
        //                value = node["value"].InnerText
        //            });
        //        }
        //    }
        //    return this.listLG;
        //}
        [HttpGet]
        public JsonResult LoadData(int page= 1, int pageSize  = 10 )
        {

            var listLG = new List<LanguageModel>();
            XmlDocument doc = new XmlDocument();

            if (selectLG == "en" || selectLG == "")
            {
                doc.Load(string.Concat(Server.MapPath(@"~/Assets/Client/data/Account.en.xml")));

                foreach (XmlNode node in doc.SelectNodes("/LocalLanguages/data "))
                {
                    listLG.Add(new LanguageModel
                    {
                        key = node.Attributes["name"].Value,
                        value = node["value"].InnerText
                    }); ;
                }
            }
            else
            {
                doc.Load(string.Concat(Server.MapPath(@"~/Assets/Client/data/Account.km.xml")));

                foreach (XmlNode node in doc.SelectNodes("/LocalLanguages/data "))
                {
                    listLG.Add(new LanguageModel
                    {
                        key = node.Attributes["name"].Value,
                        value = node["value"].InnerText
                    });
                }
            }
            var model = listLG.Skip((page - 1) * pageSize).Take(pageSize);
            int totalRow = listLG.Count;

            return Json(new
            {
                data = model,
                total = totalRow,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }
    }
}