using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ICOPlatformV1.ActionFilters
{
  public class ContractFilter : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      try
      {
        var id = filterContext.HttpContext.Session.GetString("contractid");
        if (string.IsNullOrEmpty(id))
          throw new Exception();
      }
      catch (Exception e)
      {
        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
        {
          controller = "Contract",
          action = "Index"
        }));
      }
    }
  }
}
