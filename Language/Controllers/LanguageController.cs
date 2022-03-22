using Language.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace Language.Controllers
{
    public class LanguageController : Controller
    {
        // GET: Language
        public static string selectLG = "en";

        public ActionResult Index(string ddlCulture, string Name)
        {
            return View();
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
        public JsonResult LoadData(int page = 1, int pageSize = 10)
        {

            var listLG = new List<LanguageModel>();
            XmlDocument doc = new XmlDocument();

            if (selectLG == "en" || selectLG == "")
            {
                doc.Load(string.Concat(Server.MapPath(@"~/Assets/Client/data/Account.en.xml")));
            }
            else
            {
                doc.Load(string.Concat(Server.MapPath(@"~/Assets/Client/data/Account.km.xml")));
            }

            foreach (XmlNode node in doc.SelectNodes("/LocalLanguages/data "))
            {
                listLG.Add(new LanguageModel
                {
                    key = node.Attributes["name"].Value,
                    value = node["value"].InnerText
                });
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
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(LanguageModel lg)
        {
            int check = 0;
            string path = @"/Assets/Client/data/Account.en.xml";
            XmlDocument doc = new XmlDocument();

            doc.Load(Server.MapPath(path));

            var key = lg.key;


            foreach (XmlNode node in doc.SelectNodes("/LocalLanguages/data "))
            {
                if (key == node.Attributes["name"].Value)
                {

                    check = 1;
                    break;
                }
            }
            if (check == 1)
            {
                check = 0;
                ModelState.AddModelError("", "Key đã tồn tại");
            }
            else
            {
                XmlElement ekey, evalue;
                ekey = doc.CreateElement("data");
                ekey.SetAttribute("name", lg.key.Trim());

                evalue = doc.CreateElement("value");
                evalue.InnerText = lg.value.Trim();

                ekey.AppendChild(evalue);

                doc.DocumentElement.AppendChild(ekey);
                doc.Save(Server.MapPath(path));
                return RedirectToAction("Index", "Language");
            }


            return View("Create");
        }
        [HttpPost]
        public JsonResult Delete(string key)
        {
            bool check = true;
            string path = @"/Assets/Client/data/Account.en.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath(path));

            XmlElement root = doc.DocumentElement;

            XmlNodeList list = root.ChildNodes;



            for (int i = 0; i < list.Count; i++)
            {

                if (list[i].Attributes["name"].Value == key)
                {
                    check = true;
                    root.RemoveChild(list[i]);
                }
                else
                {
                    check = false;
                }

            }
            doc.Save(Server.MapPath(path));
            return Json(new
            {
                status = check
            });
        }
        public static string pKey;

        
        public ActionResult Edit(string key ,string value)
        {
            if (key != null)
            {
                pKey = key;
            }
            var va = value;
            string path = @"/Assets/Client/data/Account.en.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath(path));

            if(value != null)
            {
                foreach (XmlNode node in doc.SelectNodes("/LocalLanguages/data "))
                {
                    if (pKey == node.Attributes["name"].Value)
                    {
                        node["value"].InnerText = value.Trim();
                    }
                }
                doc.Save(Server.MapPath(path));
                return RedirectToAction("Index", "Language");
            }

            return null;
        }
        }
}