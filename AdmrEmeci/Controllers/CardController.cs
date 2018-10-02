using AdmrEmeci.Models;
using System.Web.Mvc;
using System.Net.Mime;
using static System.Configuration.ConfigurationManager;
using AdmrEmeci.Manager.Manager;

namespace AdmrEmeci.Controllers
{
    [Authorize]
    public class CardController : BaseController
    {
        EmeciEntities Db = new EmeciEntities();


        #region Vencimiento de tarjetas
        [HttpGet]
        public ActionResult CardExpiration(int page = 1, int pageSize = 10)
        {
            var model = new ListOfPatientModel();
            model.Page = page;
            model.PageSize = pageSize;
            model.GetAllPatients();
            return View(model);
        }

        [HttpPost]
        public ActionResult CardExpiration(ListOfPatientModel model)
        {
            if (Patient.ExistPatient(model.EmeciSelected))
                model.GetAllPatients(model.EmeciSelected);
            else
            {
                model.IsError = true;
                model.Error = $"No se encontró el paciente con el #{model.EmeciSelected}";
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult CardRenovation(string emecis)
        {
            if (!string.IsNullOrEmpty(emecis))
                Card.Renovation(emecis);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Imprimir tarjeta
        [HttpGet]
        public ActionResult PrintCard()
        {
            var model = new PrintCardModel();
            model.UrlImage = $"{AppSettings["UrlFiles"]}/Images/PrintCard.jpg";
            return View(model);
        }

        [HttpPost]
        public ActionResult PrintCard(PrintCardModel model)
        {
            var q = Card.PrintCard(AppSettings["RutaImages"], AppSettings["UrlFiles"], model.EmeciSelected);
            model.Error = q.Error;
            model.IsError = q.IsError;
            model.UrlImage = q.IsError ? $"{AppSettings["UrlFiles"]}/Images/PrintCard.jpg" : q.UrlImage;
            return View(model);
        }

        public FileResult Download(string emeci)
        {
            var path = $"{AppSettings["RutaImages"]}\\{emeci}.jpg";
            var contents = System.IO.File.ReadAllBytes(path);

            var header = new ContentDisposition()
            {
                FileName = $"{emeci}.jpg",
                Inline = false
            };

            Response.AddHeader("Content-Disposition", header.ToString());
            return File(contents, "image/jpeg");
        }
        #endregion


        //public ActionResult Index()
        //{
        //    var model = new ImageCardModel();
        //    model.UrlImage = $"{AppSettings["UrlFiles"]}/Images/PrintCard.jpg";
        //    return View(model);
        //}


        //[HttpPost]
        //public ActionResult Index(ImageCardModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            bool existImage = false;
        //            foreach (string image in GetFiles(AppSettings["RutaImages"]))
        //            {
        //                string imageName = Path.GetFileName(image);
        //                if (imageName.ToLower() == $"{model.CardNumber}.jpg")
        //                    existImage = true;
        //            }


        //            if (!existImage)
        //            {
        //                var query = (from d in Db.DatosTarjeta
        //                             where d.noTarjeta == model.CardNumber
        //                             orderby d.iddatostarjeta
        //                             select new { d.Dato }).ToList();
        //                if (query.Count == 0)
        //                {
        //                    model.UrlImage = $"{AppSettings["UrlFiles"]}/Images/PrintCard.jpg";
        //                    model.Error = true;
        //                    return View(model);
        //                }

        //                Bitmap bitMapImage = new Bitmap($"{AppSettings["RutaImages"]}\\PrintCard.jpg");
        //                Graphics graphicImage = Graphics.FromImage(bitMapImage);

        //                graphicImage.SmoothingMode = SmoothingMode.AntiAlias;
        //                graphicImage.DrawString(model.CardNumber, new Font("Arial", 20, FontStyle.Bold),
        //                    SystemBrushes.WindowText, new Point(50, 175));

        //                int cont = 0, px = 545, py = 41;
        //                for (int i = 1; i <= 10; i++)
        //                {
        //                    py = 41;
        //                    for (int j = 0; j <= 9; j++)
        //                    {
        //                        graphicImage.DrawString(query[cont].Dato, new Font("Arial", 9, FontStyle.Bold), 
        //                            SystemBrushes.WindowText, new Point(px, py));

        //                        cont += 1;
        //                        py += 17;
        //                    }

        //                    px += 30;
        //                }

        //                bitMapImage.Save($"{AppSettings["RutaImages"]}\\{model.CardNumber}.jpg", ImageFormat.Jpeg);
        //                graphicImage.Dispose();
        //                bitMapImage.Dispose();
        //            }

        //            var _query = (from r in Db.Registro
        //                         where r.Emeci == model.CardNumber
        //                         select new { r.Nombre, r.Apellido }).FirstOrDefault();
        //            if (_query != null)
        //                model.PatientName = $"{_query.Nombre} {_query.Apellido}";

        //            model.UrlImage = $"{AppSettings["UrlFiles"]}/Images/{model.CardNumber}.jpg";
        //            model.CardNumberSelected = model.CardNumber;
        //            model.Error = false;
        //        }
        //        catch (Exception)
        //        {

        //        }
        //    }

        //    return View(model);
        //}






        


        



        //[HttpPost]
        //public JsonResult AutoComplete(string prefix)
        //{
        //    var query = (from r in Db.Registro
        //                 where r.Emeci.StartsWith(prefix) && r.Tipo == "P"
        //                 select new { r.Emeci }).Take(10);

        //    return Json(query, JsonRequestBehavior.AllowGet);
        //}
    }
}