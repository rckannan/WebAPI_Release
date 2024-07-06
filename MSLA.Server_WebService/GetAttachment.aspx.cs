using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using MSLA.Server.Data;
using System.Data;

namespace MSLA.Server_WebService
{
    public partial class GetAttachment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var encouded = HttpUtility.UrlEncode(HttpContext.Current.Request.QueryString["Document_ID"].ToString());
                var dat = Request.QueryString["Document_ID"].Split(new string[] { "-" }, StringSplitOptions.None);
                //ClientScript.RegisterStartupScript(this.GetType(), "Errors", "alert('" + encouded + "');", true);
                if (dat.Length == 6)
                {
                    //validate for user using session ID
                    var firstiffen = encouded.IndexOf('-');
                    var first = encouded.Remove(firstiffen);
                    var sec = encouded.Substring(firstiffen + 1, encouded.Length - firstiffen - 1);
                    //ClientScript.RegisterStartupScript(this.GetType(), "Errorsa", "alert('" + first + "');", true);
                    //ClientScript.RegisterStartupScript(this.GetType(), "Errorss", "alert('" + sec + "');", true);
                    MSLA.Server.Data.AttachedDoc doc = MSLA.Server.Data.DataConnect.getAttachment(Convert.ToInt64(first), new Guid(sec));
                    if (doc.Hasaccess)
                    {
                        Response.AddHeader("content-disposition", "attachment; filename=" + doc.FileName);
                        Response.OutputStream.Write(doc.FileData, 0, doc.FileData.Length);
                    }
                    else
                        ClientScript.RegisterStartupScript(this.GetType(), "Error1", "alert('You are not authorized to download the termsheets.');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Error2", "alert('You are not authorized to download the termsheets.');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Error3", "alert('"+ex.Message+"');", true);
            }
          
 
           
        }
    }
}