using AdmrEmeci.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Data;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System;
using AdmrEmeci.App_Code;

namespace AdmrEmeci.Controllers
{
    [Authorize]
    public class DoctorController : BaseController
    {
        EmeciEntities DB = new EmeciEntities();

        [HttpGet]
        public ActionResult List(int Page = 1, int PageSize = 10)
        {
            DoctorList Model = new DoctorList();

            PagedList<ListOfDoctor> _PagedList = new PagedList<ListOfDoctor>(GetAllDoctor(), Page, PageSize);
            Model.LDoctor = _PagedList;
            return View(Model);
        }


        [HttpPost]
        public ActionResult List(DoctorList Model)
        {
            if (ModelState.IsValid)
            {
                List<ListOfDoctor> query = (from r in DB.Registro
                                            join e in DB.Estados on r.idEstado equals e.idEstado
                                            join c in DB.Ciudades on r.idCiudad equals c.idciudad
                                            where r.Tipo == "M" && 
                                            (!string.IsNullOrEmpty(Model.CardNumber) ? r.Emeci == Model.CardNumber : r.Tipo != "P")
                                            select new ListOfDoctor()
                                            {
                                                DoctorName = r.Nombre,
                                                DoctorLastName = r.Apellido,
                                                EMECI = r.Emeci,
                                                State = e.Nombre,
                                                City = c.Nombre,
                                                Phone = r.Telefono,
                                                CellPhone = r.TelefonoCel,
                                                Email = r.Emails.Replace(",", Environment.NewLine)
                                            }).AsEnumerable().Cast<ListOfDoctor>().ToList();

                if (query.Count == 0)
                    Model.Error = true;
                PagedList<ListOfDoctor> _PagedList = new PagedList<ListOfDoctor>(query, 1, 10);
                Model.LDoctor = _PagedList;
                Model.CardNumberSelected = Model.CardNumber;
            }

            return View(Model);
        }


        [HttpPost]
        public JsonResult AutoComplete(string Prefix)
        {
            var query = (from r in DB.Registro
                         where r.Emeci.StartsWith(Prefix) && r.Tipo == "M"
                         select new { r.Emeci }).Take(10);

            return Json(query, JsonRequestBehavior.AllowGet);
        }


        public void ExportExcel()
        {
            List<ListOfDoctor> DoctorList = GetAllDoctor();
            foreach(ListOfDoctor Doctor in DoctorList)
                Doctor.Email.Replace(Environment.NewLine, ",");
            DataTable TableExcel = ConvertToDataTable(DoctorList);

            new Export().ToExcel(Response, TableExcel, "Lista_De_Doctores");
        }


        DataTable ConvertToDataTable(IList<ListOfDoctor> Data)
        {

            PropertyDescriptorCollection Properties =
                TypeDescriptor.GetProperties(typeof(ListOfDoctor));
            DataTable TableExcel = new DataTable();

            foreach (PropertyDescriptor Prop in Properties)
                TableExcel.Columns.Add(Prop.Name, Nullable.GetUnderlyingType(Prop.PropertyType) ?? Prop.PropertyType);
            foreach (ListOfDoctor Item in Data)
            {
                DataRow Row = TableExcel.NewRow();
                foreach (PropertyDescriptor Prop in Properties)
                    Row[Prop.Name] = Prop.GetValue(Item) ?? DBNull.Value;
                TableExcel.Rows.Add(Row);
            }

            TableExcel.Columns["DoctorName"].ColumnName = "Nombre del doctor";
            TableExcel.Columns["DoctorLastName"].ColumnName = "Apellido(s) del doctor";
            TableExcel.Columns["State"].ColumnName = "Estado";
            TableExcel.Columns["City"].ColumnName = "Ciudad";
            TableExcel.Columns["Phone"].ColumnName = "Teléfono(s)";
            TableExcel.Columns["CellPhone"].ColumnName = "Teléfono celular";
            TableExcel.Columns["Email"].ColumnName = "Correo electrónico";

            return TableExcel;
        }


        List<ListOfDoctor> GetAllDoctor()
        {
            List<ListOfDoctor> query = (from r in DB.Registro
                                        join e in DB.Estados on r.idEstado equals e.idEstado into es
                                        from e in es.DefaultIfEmpty()
                                        join c in DB.Ciudades on r.idCiudad equals c.idciudad into ci
                                        from c in ci.DefaultIfEmpty()
                                        where r.Tipo == "M"
                                        select new ListOfDoctor()
                                        {
                                            DoctorName = r.Nombre,
                                            DoctorLastName = r.Apellido,
                                            EMECI = r.Emeci,
                                            State = e.Nombre,
                                            City = c.Nombre,
                                            Phone = r.Telefono,
                                            CellPhone = r.TelefonoCel,
                                            Email = r.Emails.Replace(",", Environment.NewLine)
                                        }).AsEnumerable().Cast<ListOfDoctor>().ToList();
            return query;
        }
    }
}