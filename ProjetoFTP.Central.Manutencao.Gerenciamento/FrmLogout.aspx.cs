using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProjetoFTP.Web
{
    public partial class FrmLogout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Util.RegistraEntradaUsuario(this);
            Session["usuario"] = null;
            Response.Redirect("FrmLogin.aspx");
        }
    }
}