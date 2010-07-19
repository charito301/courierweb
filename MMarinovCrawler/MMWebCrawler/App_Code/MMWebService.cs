using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;

namespace Margent
{
    /// <summary>
    /// Summary description for MMWebService
    /// </summary>
    [WebService(Namespace = "MMServiceNspc")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class MMWebService : System.Web.Services.WebService
    {
        [System.Web.Services.WebMethod]
        public string[] GetSuggestions(string prefixText, int count)
        {
            using (DALWebCrawlerActive.WebCrawlerActiveDataContext dataCont = new DALWebCrawlerActive.WebCrawlerActiveDataContext(DataFetcher.ConnectionString))
            {
                return (from w in dataCont.Words.Where(w1 => w1.WordName.StartsWith(prefixText)).Take(count)
                        select w.WordName).ToArray();
            }
        }
    }
}