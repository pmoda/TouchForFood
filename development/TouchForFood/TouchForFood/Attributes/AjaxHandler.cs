using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;

namespace TouchForFood.Attributes
{
    public class AjaxAttribute : ActionMethodSelectorAttribute
    {
        private readonly bool m_isAjax;

        public AjaxAttribute(bool ajax){
            m_isAjax = ajax;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return m_isAjax == controllerContext.HttpContext.Request.IsAjaxRequest();
        }
    }
}