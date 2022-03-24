using Language.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace Language.Controllers
{
    public class LanguageController : Controller
    {
        // GET: Language
        public static string selectLG = "en";
        public List<string> listXml = new List<string>()
        {
            "en",
            "km"
        };
        public string pathXml(string x)
        {
            switch (x)
            {
                case "km":
                    return "Account.km.xml";
                default:
                    return "Account.en.xml";
            }
        }
        public ActionResult Index(string ddlCulture)
        {
            
            if (ddlCulture != null)
            {
                selectLG = ddlCulture;
            }
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
            var listLG2 = new List<LanguageModel>();
            XmlDocument doc = new XmlDocument();
            string path;
            int index = 0;
            foreach (var item in listXml)
            {
                path = @"/Assets/Client/data/"+pathXml(item);
                doc.Load(string.Concat(Server.MapPath(path)));
                if (index == 0)
                {
                    foreach (XmlNode node in doc.SelectNodes("/LocalLanguages/data "))
                    {
                        listLG.Add(new LanguageModel
                        {
                            key = node.Attributes["name"].Value,
                            value = node["value"].InnerText
                        });

                    }
                }
                else
                {
                    foreach (XmlNode node in doc.SelectNodes("/LocalLanguages/data "))
                    {
                        listLG2.Add(new LanguageModel
                        {
                            key = node.Attributes["name"].Value,
                            value = node["value"].InnerText
                        });
                    }
                }
                index++;
            }
            var model = listLG.Skip((page - 1) * pageSize).Take(pageSize);
            var model2 = listLG2.Skip((page - 1) * pageSize).Take(pageSize);
            int totalRow = listLG.Count;
            return Json(new
            {
                data = model,
                data2 = model2,
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
            XmlDocument doc = new XmlDocument();
            var path = @"/Assets/Client/data/"+pathXml(selectLG);
            doc.Load(Server.MapPath(path));

            foreach (XmlNode node in doc.SelectNodes("/LocalLanguages/data "))
            {
                if (lg.key == node.Attributes["name"].Value)
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
                foreach (var item in listXml)
                {
                    path = @"/Assets/Client/data/" +pathXml(item);
                    doc.Load(Server.MapPath(path));
                    XmlElement ekey, evalue;
                    ekey = doc.CreateElement("data");
                    ekey.SetAttribute("name", lg.key.Trim());
                    evalue = doc.CreateElement("value");
                    var s = selectLG;
                    evalue.InnerText = TranslateText(lg.value.Trim(), selectLG, item);

                    ekey.AppendChild(evalue);
                    doc.DocumentElement.AppendChild(ekey);
                    doc.Save(Server.MapPath(path));
                }
                return RedirectToAction("Index", "Language");
            }
            return View("Create");
        }

        [HttpPost]
        public JsonResult Delete(string key)
        {
            bool check = true;
            string path; 
            XmlDocument doc = new XmlDocument();


            foreach (var item in listXml)
            {
                path = @"/Assets/Client/data/"+pathXml(item);
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
            }
            return Json(new
            {
                status = check
            });
        }
        //public static string pKey;
        //public ActionResult Edit(string key ,string value)
        //{
        //    if (key != null)
        //    {
        //        pKey = key;
        //    }

        //    var path = @"/Assets/Client/data/" + pathXml(selectLG);
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(Server.MapPath( path));

        //    if(!string.IsNullOrEmpty(value))
        //    {
        //        foreach (XmlNode node in doc.SelectNodes("/LocalLanguages/data "))
        //        {
        //            if (pKey == node.Attributes["name"].Value)
        //            {
        //                node["value"].InnerText = value.Trim();
        //            }
        //        }
        //        doc.Save(Server.MapPath(path));
        //        return RedirectToAction("Index", "Language");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Bạn chưa nhập nội dung");
        //        return View();
        //    }
        //}

        public static string pKey;

        [HttpPost]
        public ActionResult Edit(string key, string valueNew)
        {
            valueNew = valueNew.Trim();
            //do whatever
            if (key != null)
            {
                pKey = key;
            }

            string path;
            XmlDocument doc = new XmlDocument();

            foreach (var item in listXml)
            {
                path = @"/Assets/Client/data/" + pathXml(item);
                doc.Load(Server.MapPath(path));
                foreach (XmlNode node in doc.SelectNodes("/LocalLanguages/data"))
                {
                    if (pKey == node.Attributes["name"].Value && valueNew != node["value"].InnerText)
                    {
                        node["value"].InnerText = TranslateText( valueNew,"en", item);
                    }
                }
                doc.Save(Server.MapPath(path));
            }
            return RedirectToAction("Index", "Language");
        }
        /// <summary>
        /// Hàm dùng để dịch văn bản
        /// Các tham số chuyền vào: văn bản,lgInput: ngôn ngữ hiện tại, lgOutput: ngôn ngữ muốn dịch
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string TranslateText(string input, string lgInput= "en",string lgOutput="km")
        {
            if(lgInput != lgOutput)
            {
                string url = String.Format
            ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
             lgInput, lgOutput, Uri.EscapeUriString(input));
                HttpClient httpClient = new HttpClient();
                string result = httpClient.GetStringAsync(url).Result;
                var jsonData = new JavaScriptSerializer().Deserialize<List<dynamic>>(result);
                var translationItems = jsonData[0];
                string translation = "";
                foreach (object item in translationItems)
                {
                    IEnumerable translationLineObject = item as IEnumerable;
                    IEnumerator translationLineString = translationLineObject.GetEnumerator();
                    translationLineString.MoveNext();
                    translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
                }
                if (translation.Length > 1) { translation = translation.Substring(1); };
                return translation;
            }
            return input;
        }
    }
}