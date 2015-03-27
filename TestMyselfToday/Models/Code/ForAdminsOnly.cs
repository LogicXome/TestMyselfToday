using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestMyselfToday.Models.Code
{
    public class ForAdminsOnly : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if(httpContext.Session["IsTestMyTodayAdmin"] != null)
            {
                return Convert.ToBoolean(httpContext.Session["IsTestMyTodayAdmin"]);
            }
            return false;
        }
    }

}