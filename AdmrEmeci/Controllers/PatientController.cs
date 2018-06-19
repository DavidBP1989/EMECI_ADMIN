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
        EmeciEntities DB = new EmeciEntities();

        [HttpGet]
        public ActionResult List(int Page = 1, int PageSize = 10)
        {
            PatientList Model = new PatientList();

            PagedList<ListOfPatient> _PagedList = new PagedList<ListOfPatient>(GetAllPatients(), Page, PageSize);
            Model.LPatient = _PagedList;
            return View(Model);
        }


        [HttpPost]
        public ActionResult List(PatientList Model)
        {
            if (ModelState.IsValid)
            {
                List<ListOfPatient> query = (from r in DB.Registro
                                            join e in DB.Estados on r.idEstado equals e.idEstado into es
                                            from e in es.DefaultIfEmpty()
                                            join c in DB.Ciudades on r.idCiudad equals c.idciudad into ci
                                            from c in ci.DefaultIfEmpty()
                                            where r.Tipo == "P" &&
                                            (!string.IsNullOrEmpty(Model.CardNumber) ? r.Emeci == Model.CardNumber : r.Tipo != "P")
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
                                            }).AsEnumerable().Cast<ListOfPatient>().ToList();

                if (query.Count == 0)
                    Model.Error = true;
                PagedList<ListOfPatient> _PagedList = new PagedList<ListOfPatient>(query, 1, 10);
                Model.LPatient = _PagedList;
                Model.CardNumberSelected = Model.CardNumber;
            }

            return View(Model);
        }


        [HttpPost]
        public JsonResult AutoComplete(string Prefix)
        {
            var query = (from r in DB.Registro
                         where r.Emeci.StartsWith(Prefix) && r.Tipo == "P"
                         select new { r.Emeci }).Take(10);

            return Json(query, JsonRequestBehavior.AllowGet);
        }



        public void ExportExcel()
        {
            List<ListOfPatient> PatientList = GetAllPatients();
            DataTable TableExcel = ConvertToDataTable(PatientList);

            new Export().ToExcel(Response, TableExcel, "Lista_De_Pacientes");
        }


        DataTable ConvertToDataTable(IList<ListOfPatient> Data)
        {

            PropertyDescriptorCollection Properties =
                TypeDescriptor.GetProperties(typeof(ListOfPatient));
            DataTable TableExcel = new DataTable();

            foreach (PropertyDescriptor Prop in Properties)
            {
                if (Prop.PropertyType == typeof(DateTime?))
                    TableExcel.Columns.Add(Prop.Name, typeof(string));
                else TableExcel.Columns.Add(Prop.Name, Nullable.GetUnderlyingType(Prop.PropertyType) ?? Prop.PropertyType);
            }
            foreach (ListOfPatient Item in Data)
            {
                DataRow Row = TableExcel.NewRow();
                foreach (PropertyDescriptor Prop in Properties)
                {
                    if (Prop.Name == "ActivationDate" || Prop.Name == "DueDate")
                    {
                        DateTime? Date = Convert.ToDateTime(Prop.GetValue(Item) ?? (DateTime?)null);
                        Row[Prop.Name] = Date.HasValue ? Date.Value.ToString("dd/MM/yyyy") : string.Empty;
                    }
                    else Row[Prop.Name] = Prop.GetValue(Item) ?? DBNull.Value;
                }
                TableExcel.Rows.Add(Row);
            }

            TableExcel.Columns["PatientName"].ColumnName = "Nombre del doctor";
            TableExcel.Columns["PatientLastName"].ColumnName = "Apellido(s) del doctor";
            TableExcel.Columns["State"].ColumnName = "Estado";
            TableExcel.Columns["City"].ColumnName = "Ciudad";
            TableExcel.Columns["Phone"].ColumnName = "Teléfono(s)";
            TableExcel.Columns["CellPhone"].ColumnName = "Teléfono celular";
            TableExcel.Columns["Email"].ColumnName = "Correo electrónico";
            TableExcel.Columns["ActivationDate"].ColumnName = "Fecha de registro";
            TableExcel.Columns["DueDate"].ColumnName = "Fecha de vencimiento";

            return TableExcel;
        }


        List<ListOfPatient> GetAllPatients()
        {
            List<ListOfPatient> query = (from r in DB.Registro
                                         join e in DB.Estados on r.idEstado equals e.idEstado into es
                                         from e in es.DefaultIfEmpty()
                                         join c in DB.Ciudades on r.idCiudad equals c.idciudad into ci
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
                                        }).AsEnumerable().Cast<ListOfPatient>().ToList();
            return query;
        }
    }
}