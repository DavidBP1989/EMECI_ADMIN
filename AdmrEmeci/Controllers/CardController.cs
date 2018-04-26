using AdmrEmeci.Models;
using System.Web.Mvc;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Mime;
using System.IO;
using static System.IO.Directory;
using static System.Configuration.ConfigurationManager;

namespace AdmrEmeci.Controllers
{
    public class CardController : Controller
    {
        EmeciEntities DB = new EmeciEntities();

        public ActionResult Index()
        {
            var Model = new ImageCardModel();
            Model.UrlImage = $"{AppSettings["UrlFiles"]}/Images/PrintCard.jpg";
            return View(Model);
        }


        [HttpPost]
        public ActionResult Index(ImageCardModel Model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool ExistImage = false;
                    foreach (string Image in GetFiles(AppSettings["RutaImages"]))
                    {
                        string ImageName = Path.GetFileName(Image);
                        if (ImageName.ToLower() == $"{Model.CardNumber}.jpg")
                            ExistImage = true;
                    }


                    if (!ExistImage)
                    {
                        var query = (from d in DB.DatosTarjeta
                                     where d.noTarjeta == Model.CardNumber
                                     orderby d.iddatostarjeta
                                     select new { d.Dato }).ToList();
                        if (query.Count == 0)
                        {
                            Model.UrlImage = $"{AppSettings["UrlFiles"]}/Images/PrintCard.jpg";
                            Model.Error = true;
                            return View(Model);
                        }

                        Bitmap BitMapImage = new Bitmap($"{AppSettings["RutaImages"]}\\PrintCard.jpg");
                        Graphics GraphicImage = Graphics.FromImage(BitMapImage);

                        GraphicImage.SmoothingMode = SmoothingMode.AntiAlias;
                        GraphicImage.DrawString(Model.CardNumber, new Font("Arial", 20, FontStyle.Bold),
                            SystemBrushes.WindowText, new Point(50, 175));

                        int cont = 0, px = 545, py = 41;
                        for (int i = 1; i <= 10; i++)
                        {
                            py = 41;
                            for (int j = 0; j <= 9; j++)
                            {
                                GraphicImage.DrawString(query[cont].Dato, new Font("Arial", 9, FontStyle.Bold), 
                                    SystemBrushes.WindowText, new Point(px, py));

                                cont += 1;
                                py += 17;
                            }

                            px += 30;
                        }

                        BitMapImage.Save($"{AppSettings["RutaImages"]}\\{Model.CardNumber}.jpg", ImageFormat.Jpeg);
                        GraphicImage.Dispose();
                        BitMapImage.Dispose();
                    }

                    var _query = (from r in DB.Registro
                                 where r.Emeci == Model.CardNumber
                                 select new { r.Nombre, r.Apellido }).FirstOrDefault();
                    if (_query != null)
                        Model.PatientName = $"{_query.Nombre} {_query.Apellido}";

                    Model.UrlImage = $"{AppSettings["UrlFiles"]}/Images/{Model.CardNumber}.jpg";
                    Model.CardNumberSelected = Model.CardNumber;
                    Model.Error = false;
                }
                catch (Exception)
                {

                }
            }

            return View(Model);
        }




        public FileResult Download(string CardNumber)
        {
            var Path = $"{AppSettings["RutaImages"]}\\{CardNumber}.jpg";
            var Contents = System.IO.File.ReadAllBytes(Path);

            var Header = new ContentDisposition()
            {
                FileName = $"{CardNumber}.jpg",
                Inline = false
            };

            Response.AddHeader("Content-Disposition", Header.ToString());
            return File(Contents, "image/jpeg");
        }


        



        [HttpPost]
        public JsonResult AutoComplete(string Prefix)
        {
            var query = (from r in DB.Registro
                         where r.Emeci.StartsWith(Prefix) && r.Tipo == "P"
                         select new { r.Emeci }).Take(10);

            return Json(query, JsonRequestBehavior.AllowGet);
        }
    }
}