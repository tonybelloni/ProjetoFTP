using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProjetoFTP.Utilidades;

namespace ProjetoFTP.Web
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Util.VerificaSessaoUsuario(this, "usuario");
            Util.AdicionaInfoUsuarioNaTela(page_bar);
            Util.RegistraEntradaUsuario(this);

        }
    }
}