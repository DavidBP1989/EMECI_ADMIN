using AdmrEmeci.Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static AdmrEmeci.Manager.Models.PatientModel;

namespace AdmrEmeci.Manager.Manager
{
    public class Patient
    {
        public static List<PatientModel> GetPatients(string emeci)
        {
            return GetListPatients(emeci);
        }

        public static List<PatientModel> GetPatients()
        {
            return GetListPatients();
        }

        static List<PatientModel> GetListPatients(string emeci = "")
        {
            using (var dB = new Entities())
            {
                var q = (from r in dB.Registro
                         join e in dB.Estados on r.idEstado equals e.idEstado
                         into es
                         from e in es.DefaultIfEmpty()
                         join c in dB.Ciudades on r.idCiudad equals c.idciudad
                         into ci
                         from c in ci.DefaultIfEmpty()
                         where r.Tipo == "P"
                         select new PatientModel
                         {
                             Name = r.Nombre,
                             LastName = r.Apellido,
                             Emeci = r.Emeci,
                             City = c.Nombre,
                             State = e.Nombre,
                             CellPhone = r.TelefonoCel,
                             Phone = r.Telefono,
                             Email = r.Emails,
                             ActivationDate = r.FechaRegistro,
                             DueDate = r.FechaExpiracion,
                             RenovationDate = r.FechaRenovacion
                         }).OrderByDescending(x => x.ActivationDate);

                if (emeci != string.Empty)
                    q = q.Where(x => x.Emeci == emeci).OrderByDescending(x => x.ActivationDate);

                List<PatientModel> response = q.ToList().Aggregate(
                new List<PatientModel>(),
                (x, y) =>
                {
                    y.StatusDateCard = GetStatusCard(y.DueDate);
                    x.Add(y);
                    return x;
                });

                return response;
            }
        }

        public static bool ExistPatient(string emeci)
        {
            return new Entities().Registro.Any(x => x.Emeci == emeci);
        }

        static StatusDate GetStatusCard(DateTime? dueDate)
        {
            if (dueDate.HasValue)
            {
                if (DateTime.Compare(dueDate.Value, DateTime.Now) < 0)
                    return StatusDate.Vencida;
            }

            return StatusDate.Vigente;
        }
    }
}
