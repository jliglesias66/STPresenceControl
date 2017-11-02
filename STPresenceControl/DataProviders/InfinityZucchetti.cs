using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STPresenceControl.Models;
using STPresenceControl.Common;
using HtmlAgilityPack;
using System.Web;
using System.Net.Http;
using System.Globalization;
using Newtonsoft.Json.Linq;
using STPresenceControl.Enums;
using System.Net;

namespace STPresenceControl.DataProviders
{
    public class InfinityZucchetti : IDataProvider
    {
        private readonly Http _http;

        public InfinityZucchetti()
        {
            _http = new Http();
        }

        public async Task LoginAsync(string username, string password)
        {
            var result = await _http.PostAsync(
                    new Uri("https://saas.hrzucchetti.it/hrpsolmicro/servlet/cp_login"),
                    new StringContent(String.Format("m_cUserName={0}&m_cPassword={1}&w_Modal=N&wSHOWSENDMYPWD=true&mylink=M&m_cFailedLoginReason=&ssotrust=&GWINLOGON=&g_codute=0.0&m_cAction=login&m_cURL=&m_cURLOnError=jsp%2Flogin.jsp&error=0&m_cForceLogin=&w_FirstCodAzi=001&g_UserCode=-1&g_UserName=&ssoStatus=0&m_cInstance=&m_cCaptcha=&g_codazi=001&Nodes=t&memo=%2C&TITOLO=f&GLOGOLGINURL=..%2Floghi%2Flogin_solmicro.png&ERM_GANVERATT=070500&mylang=&browserlang=&GLOGOLOGIN=&g_UserLang=&GLANGUAGEINSTALL=%3BDEU%7Cdeutsch%7C..%2Fimages%2Fflag%2FGermany.png%3BENG%7CEnglish%7C..%2Fimages%2Fflag%2FUnitedKingdom.png%3BFRA%7Cfrancais%7C..%2Fimages%2Fflag%2FFrance.png%3BITA%7CItaliano%7C..%2Fimages%2Fflag%2FItaly.png%3BPOR%7Cportuguese%7C..%2Fimages%2Fflag%2FPortugal.png%3BRON%7Cromanian%7C..%2Fimages%2Fflag%2Fdefault.png%3BSPA%7CEspa%C3%B1ol%7C..%2Fimages%2Fflag%2FSpain.png&GFLSENDMYPWD=S&GERMNAME=HRPortal&GLOGINTITLECO=&GIDLANGUAGE=ITA",
                                        WebUtility.UrlEncode(username), WebUtility.UrlEncode(password)),
                                    Encoding.ASCII,
                                    "application/x-www-form-urlencoded")
                    );

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(result);

            //html esperado: <meta http-equiv="refresh" content='3;url=../../hrpsolmicro/servlet/cp_login?m_cParameterCache=puajemrshn&amp;m_cDontLoop=prpfstyrvd' />
            var cpLoginQueryString = htmlDoc.DocumentNode.SelectSingleNode("//meta[@http-equiv='refresh']").Attributes["content"].Value.Split('?')[1];

            cpLoginQueryString = HttpUtility.HtmlDecode(cpLoginQueryString);

            await _http.GetAsync(new Uri(String.Format("https://saas.hrzucchetti.it/hrpsolmicro/servlet/cp_login?{0}", cpLoginQueryString)));
        }
        public async Task<List<PresenceControlEntry>> GetPrensenceControlEntriesAsync(DateTime date)
        {
            var response = await _http.PostAsync(
                     new Uri("https://saas.hrzucchetti.it/hrpsolmicro/servlet/SQLDataProviderServer"),
                     new StringContent(String.Format("rows=10&startrow=0&count=true&&sqlcmd=rows%3Aushp_fgettimbrus&pDATE={0}", date.ToString("yyyy-MM-dd")),
                                     Encoding.UTF8,
                                     "application/x-www-form-urlencoded")
                     );

            var res = JObject.Parse(response);

            var presenceControlEntries = new List<PresenceControlEntry>();

            if (res is JObject resObj)
            {
                var jobectData = resObj.SelectToken("Data").Children().Where(r => r.GetType() == typeof(JArray));

                var fields = resObj.SelectToken("Fields").Children().Values<string>().ToArray();

                var index_DAYSTAMP = Array.IndexOf(fields, "DAYSTAMP");
                var index_TIMETIMBR = Array.IndexOf(fields, "TIMETIMBR");
                var index_DIRTIMBR = Array.IndexOf(fields, "DIRTIMBR");

                presenceControlEntries =
                (from r in jobectData
                 select new PresenceControlEntry
                 {
                     Date = DateTime.ParseExact(String.Format("{0} {1}", r[index_DAYSTAMP], r[index_TIMETIMBR]), "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture),
                     Type = GetPresenceControlEntryType(r[index_DIRTIMBR].ToString())
                 }).OrderBy(e => e.Date).ToList();

            }

            return presenceControlEntries;
        }

        private PresenceControlEntryTypeEnum? GetPresenceControlEntryType(string type)
        {
            switch (type.ToUpper())
            {
                case "E":
                    return PresenceControlEntryTypeEnum.Entry;
                case "U":
                    return PresenceControlEntryTypeEnum.Exit;
                default:
                    return null;
            }
        }
    }
}
