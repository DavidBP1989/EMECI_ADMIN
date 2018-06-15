using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Drawing;
using System.Web;

namespace AdmrEmeci.App_Code
{
    public class Export
    {
        public void ToExcel(HttpResponseBase Response, DataTable Table, string FileName)
        {
            using(ExcelPackage Excel = new ExcelPackage())
            {
                ExcelWorksheet WS = Excel.Workbook.Worksheets.Add(FileName);
                WS.Cells.AutoFitColumns();
                WS.Cells["A1"].LoadFromDataTable(Table, true);
                string Range = string.Empty;
                switch (FileName)
                {
                    case "Lista_De_Doctores":
                        Range = "A1:H1";
                        break;
                    case "Lista_De_Pacientes":
                        Range = "A1:J1";
                        break;
                }
                using(ExcelRange Rng = WS.Cells[Range])
                {
                    Rng.AutoFitColumns();
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    Rng.Style.Font.Bold = true;
                    Rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    Rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
                    Rng.Style.Font.Color.SetColor(Color.White);
                }

                Response.Clear();
                Response.AddHeader("content-disposition", $"attachment; filename={FileName}.xlsx");
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.BinaryWrite(Excel.GetAsByteArray());
                Response.End();
            }
        }
    }
}