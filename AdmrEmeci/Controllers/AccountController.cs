using AdmrEmeci.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace AdmrEmeci.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        EmeciEntities DB = new EmeciEntities();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult LogIn()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View(new LoginModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogIn(LoginModel Model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var IsValid = (from u in DB.Usuarios
                               where u.IdUsuario == Model.User &&
                               u.Password == Model.Password
                               select u).Any();
                if (IsValid)
                {
                    FormsAuthentication.SetAuthCookie(Model.User, Model.RememberMe);
                    if (!string.IsNullOrEmpty(ReturnUrl))
                        return Redirect(ReturnUrl);
                    else return RedirectToAction("Index", "Home");
                }
                else Model.Error = "Usuario o contraseña incorrecto";
            }
            else Model.Error = "El usuario y contraseña son necesario para el inicio de sesión";

            return View(Model);
        }


        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("LogIn", "Account");
        }
    }
}