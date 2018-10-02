using AdmrEmeci.App_Code;
using AdmrEmeci.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace AdmrEmeci.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        EmeciEntities Db = new EmeciEntities();

        [HttpGet]
        public ActionResult List(int page = 1, int pageSize = 10)
        {
            PatientList model = new PatientList();

            PagedList<ListOfPatient> pagedList = new PagedList<ListOfPatient>(GetAllPatients(), page, pageSize);
            model.LPatient = pagedList;
            return View(model);
        }


        [HttpPost]
        public ActionResult List(PatientList model)
        {
            if (ModelState.IsValid)
            {
                PagedList<ListOfPatient> pagedList;
                if (model.CardNumber == null)
                {
                    pagedList = new PagedList<ListOfPatient>(GetAllPatients(), 1, 10);
                    model.LPatient = pagedList;
                    model.Error = false;
                    return View(model);
                }

                List<ListOfPatient> query = (from r in Db.Registro
                                            join e in Db.Estados on r.idEstado equals e.idEstado into es
                                            from e in es.DefaultIfEmpty()
                                            join c in Db.Ciudades on r.idCiudad equals c.idciudad into ci
                                            from c in ci.DefaultIfEmpty()
                                            where r.Tipo == "P" &&
                                            (!string.IsNullOrEmpty(model.CardNumber) ? r.Emeci == model.CardNumber : r.Tipo != "P")
                                            select new ListOfPatient()
                                            {
                                                PatientName = r.Nombre,
                                                PatientLastName = r.Apellido,
                                                EMECI = r.Emeci,
                                                State = e.Nombre,
                                                City = c.Nombre,
                                                Phone = r.Telefono,
                                                CellPhone = r.TelefonoCel,
                                                Email = r.Emails.Replace(",", Environment.NewLine),
                                                ActivationDate = r.FechaRegistro.HasValue ? r.FechaRegistro.Value : (DateTime?)null,
                                                DueDate = r.FechaExpiracion.HasValue ? r.FechaExpiracion.Value : (DateTime?)null
                                            }).OrderByDescending(x => x.ActivationDate)
                                            .AsEnumerable().Cast<ListOfPatient>().ToList();

                if (query.Count == 0)
                    model.Error = true;
                pagedList = new PagedList<ListOfPatient>(query, 1, 10);
                model.LPatient = pagedList;
                model.CardNumberSelected = model.CardNumber;
            }

            return View(model);
        }


        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            var query = (from r in Db.Registro
                         where r.Emeci.StartsWith(prefix) && r.Tipo == "P"
                         select new { r.Emeci }).Take(10);

            return Json(query, JsonRequestBehavior.AllowGet);
        }



        public void ExportExcel()
        {
            List<ListOfPatient> patientList = GetAllPatients();
            DataTable tableExcel = ConvertToDataTable(patientList);

            new Export().ToExcel(Response, tableExcel, "Lista_De_Pacientes");
        }


        DataTable ConvertToDataTable(IList<ListOfPatient> data)
        {

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(ListOfPatient));
            DataTable tableExcel = new DataTable();

            foreach (PropertyDescriptor prop in properties)
            {
                if (prop.PropertyType == typeof(DateTime?))
                    tableExcel.Columns.Add(prop.Name, typeof(string));
                else tableExcel.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (ListOfPatient item in data)
            {
                DataRow row = tableExcel.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (prop.Name == "ActivationDate" || prop.Name == "DueDate")
                    {
                        DateTime? date = Convert.ToDateTime(prop.GetValue(item) ?? (DateTime?)null);
                        row[prop.Name] = date.HasValue ? date.Value.ToString("dd/MM/yyyy") : string.Empty;
                    }
                    else row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                tableExcel.Rows.Add(row);
            }

            tableExcel.Columns["PatientName"].ColumnName = "Nombre del doctor";
            tableExcel.Columns["PatientLastName"].ColumnName = "Apellido(s) del doctor";
            tableExcel.Columns["State"].ColumnName = "Estado";
            tableExcel.Columns["City"].ColumnName = "Ciudad";
            tableExcel.Columns["Phone"].ColumnName = "Teléfono(s)";
            tableExcel.Columns["CellPhone"].ColumnName = "Teléfono celular";
            tableExcel.Columns["Email"].ColumnName = "Correo electrónico";
            tableExcel.Columns["ActivationDate"].ColumnName = "Fecha de registro";
            tableExcel.Columns["DueDate"].ColumnName = "Fecha de vencimiento";

            return tableExcel;
        }


        List<ListOfPatient> GetAllPatients()
        {
            List<ListOfPatient> query = (from r in Db.Registro
                                         join e in Db.Estados on r.idEstado equals e.idEstado into es
                                         from e in es.DefaultIfEmpty()
                                         join c in Db.Ciudades on r.idCiudad equals c.idciudad into ci
                                         from c in ci.DefaultIfEmpty()
                                         where r.Tipo == "P"
                                         select new ListOfPatient()
                                         {
                                             PatientName = r.Nombre,
                                             PatientLastName = r.Apellido,
                                             EMECI = r.Emeci,
                                             State = e.Nombre,
                                             City = c.Nombre,
                                             Phone = r.Telefono,
                                             CellPhone = r.TelefonoCel,
                                             Email = r.Emails.Replace(",", Environment.NewLine),
                                             ActivationDate = r.FechaRegistro.HasValue ? r.FechaRegistro.Value : (DateTime?)null,
                                             DueDate = r.FechaExpiracion.HasValue ? r.FechaExpiracion.Value : (DateTime?)null
                                         }).OrderByDescending(x => x.ActivationDate)
                                         .AsEnumerable().Cast<ListOfPatient>().ToList();
            return query;
        }
    }
}