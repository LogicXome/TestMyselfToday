using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestMyselfToday.Models.Code
{
    public class ForUsers : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["IsValueSet"] == null)
            {
                httpContext.Session["TestMyselfToday"] = "Test Myself Today";

                httpContext.Session["TestNew"] = "New Tests";
                httpContext.Session["TestBest"] = "Best Tests";
                httpContext.Session["TestRandom"] = "Random Test";

                httpContext.Session["TermsConditions"] = "Terms and conditions";
                httpContext.Session["Privacy"] = "Privacy";
                httpContext.Session["SocialFacebook"] = "Facebook";

                httpContext.Session["TestOther"] = "Other Tests";
                httpContext.Session["TestStart"] = "Start";
                httpContext.Session["TestAgain"] = "Test Again";

                var hostStr = httpContext.Request.Url.Host.Split('.');

                long languageId = 0;


                TMTEntities db = new TMTEntities();

                if (hostStr.Length > 0)
                {
                    string languageShortCode = hostStr[0];

                    if (languageShortCode.Length == 2)
                    {

                        var cdItem = db.CommonDictionaries.Where(x => x.CDKey.Equals("LanguageShortCode", StringComparison.OrdinalIgnoreCase)
                                && x.CDValue.Equals(languageShortCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                        if (cdItem != null)
                        {
                            

                            languageId = cdItem.LanguageId;
                        }
                    }
                }

                if (languageId == 0)
                {
                    var langauge = db.Languages.OrderBy(o => o.SortOrder).FirstOrDefault();
                    if (langauge != null)
                    {
                        languageId = langauge.Id;
                    }
                }

                if (languageId > 0)
                {
                    httpContext.Session["SelectedLanguage"] = languageId;
                    httpContext.Session["IsValueSet"] = true;

                    var lstCommonDictionary = db.CommonDictionaries.Where(x => x.LanguageId == languageId).ToList();

                    var cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Test-Myself-Today", StringComparison.OrdinalIgnoreCase));

                    if (cd != null)
                    {
                        httpContext.Session["TestMyselfToday"] = cd.CDValue;
                    }

                    cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Test-New", StringComparison.OrdinalIgnoreCase));

                    if (cd != null)
                    {
                        httpContext.Session["TestNew"] = cd.CDValue;
                    }

                    cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Test-Best", StringComparison.OrdinalIgnoreCase));

                    if (cd != null)
                    {
                        httpContext.Session["TestBest"] = cd.CDValue;
                    }

                    cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Test-Random", StringComparison.OrdinalIgnoreCase));

                    if (cd != null)
                    {
                        httpContext.Session["TestRandom"] = cd.CDValue;
                    }

                    cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Terms-Conditions", StringComparison.OrdinalIgnoreCase));

                    if (cd != null)
                    {
                        httpContext.Session["TermsConditions"] = cd.CDValue;
                    }

                    cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Privacy", StringComparison.OrdinalIgnoreCase));

                    if (cd != null)
                    {
                        httpContext.Session["Privacy"] = cd.CDValue;
                    }

                    cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Social-Facebook", StringComparison.OrdinalIgnoreCase));

                    if (cd != null)
                    {
                        httpContext.Session["SocialFacebook"] = cd.CDValue;
                    }

                    cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Test-Other", StringComparison.OrdinalIgnoreCase));

                    if (cd != null)
                    {
                        httpContext.Session["TestOther"] = cd.CDValue;
                    }

                    cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Test-Start", StringComparison.OrdinalIgnoreCase));

                    if (cd != null)
                    {
                        httpContext.Session["TestStart"] = cd.CDValue;
                    }

                    cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Test-Again", StringComparison.OrdinalIgnoreCase));

                    if (cd != null)
                    {
                        httpContext.Session["TestAgain"] = cd.CDValue;
                    }
                }
            }

            return true;
        }
    }
}