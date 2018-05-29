using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AdmrEmeci.App_Code
{
    public class Export
    {
        public void ToExcel(HttpResponseBase Response, object Table, string FileName)
        {
            var Grid = new GridView();
            Grid.DataSource = Table;
            Grid.DataBind();

            Response.ClearContent();
            Response.AddHeader("content-disposition", $"attachment; filename={FileName}.xls");
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            Grid.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }
    }
}