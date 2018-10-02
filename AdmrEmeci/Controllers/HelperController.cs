using System.Web.Mvc;
using AdmrEmeci.Manager.Helper;

namespace AdmrEmeci.Controllers
{
    public class HelperController : Controller
    {
        [HttpPost]
        public JsonResult Autocomplete(string typeSearch, string prefix)
        {
            var type = new Seeker.TypeOfSearch();
            switch (typeSearch.ToLower())
            {
                case "byname":
                    type = Seeker.TypeOfSearch.byName;
                    break;
                case "bynumber":
                    type = Seeker.TypeOfSearch.byNumber;
                    break;
            }

            var result = Seeker.Autocomplete(type, prefix);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}