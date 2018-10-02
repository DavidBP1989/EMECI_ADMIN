using AdmrEmeci.Manager.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using static System.IO.Directory;

namespace AdmrEmeci.Manager.Manager
{
    public class Card
    {
        public static void Renovation(string emeciCards)
        {
            using (var dB = new Entities())
            {
                foreach(string emeci in emeciCards.Split(','))
                {
                    var q = dB.Registro.SingleOrDefault(x => x.Emeci == emeci);
                    if (q != null)
                    {
                        if (q.FechaExpiracion.HasValue)
                        {
                            q.FechaRenovacion = DateTime.Now.Date;
                            if (DateTime.Compare(DateTime.Now, q.FechaExpiracion.Value) < 0)
                                q.FechaExpiracion = q.FechaExpiracion.Value.AddYears(1).Date;
                            else if (DateTime.Compare(DateTime.Now, q.FechaExpiracion.Value) > 0)
                                q.FechaExpiracion = DateTime.Now.Date.AddYears(1).Date;
                            dB.SaveChanges();
                        }
                    }
                }
            }
        }

        public static CardModel PrintCard(string routeImages, string urlFiles, string emeci)
        {
            var model = new CardModel();
            try
            {
                bool existImage = GetFiles(routeImages).Any(x => Path.GetFileName(x).ToLower() == (emeci + ".jpg"));

                if (!existImage)
                {
                    using(var dB = new Entities())
                    {
                        var q = dB.DatosTarjeta
                            .Where(x => x.noTarjeta == emeci)
                            .OrderBy(x => x.iddatostarjeta)
                            .Select(x => new { x.Dato }).ToList();
                        if (q.Count > 0)
                        {
                            Bitmap bitMapImage = new Bitmap($"{routeImages}\\PrintCard.jpg");
                            Graphics graphicImage = Graphics.FromImage(bitMapImage);

                            graphicImage.SmoothingMode = SmoothingMode.AntiAlias;
                            graphicImage.DrawString(emeci, new Font("Arial", 20, FontStyle.Bold),
                                SystemBrushes.WindowText, new Point(50, 175));

                            int cont = 0, px = 545, py = 41;
                            for (int i = 1; i <= 10; i++)
                            {
                                py = 41;
                                for (int j = 0; j <= 9; j++)
                                {
                                    graphicImage.DrawString(q[cont].Dato, new Font("Arial", 9, FontStyle.Bold),
                                        SystemBrushes.WindowText, new Point(px, py));

                                    cont += 1;
                                    py += 17;
                                }

                                px += 30;
                            }

                            bitMapImage.Save($"{routeImages}\\{emeci}.jpg", ImageFormat.Jpeg);
                            graphicImage.Dispose();
                            bitMapImage.Dispose();
                        }
                        else
                        {
                            model.IsError = true;
                            model.Error = "Datos de la tarjeta no encontrada";
                        }
                    }
                }

                model.UrlImage = $"{urlFiles}/Images/{emeci}.jpg";
            }
            catch (Exception ex)
            {
                model.IsError = true;
                model.Error = $"Error al obtener la tarjeta: {ex.Message}";
            }

            return model;
        }
    }
}
