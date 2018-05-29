using System.Web.Mvc;

namespace AdmrEmeci.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ViewData["uname"] = User.Identity.Name;
        }
    }
}