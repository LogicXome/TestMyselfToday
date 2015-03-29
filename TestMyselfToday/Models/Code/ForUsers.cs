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
            }

            return true;
        }
    }
}