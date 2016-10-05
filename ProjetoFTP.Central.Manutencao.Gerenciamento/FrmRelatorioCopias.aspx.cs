using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProjetoFTP.Utilidades;
using System.Data;

namespace ProjetoFTP.Web
{
    public partial class FrmRelatorioCopias : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Util.VerificaSessaoUsuario(this, "usuario");
            Util.VerificaPermissao(this, new string[] { CamadaConfiguracao.PERMISSAO_ADMINISTRADOR, CamadaConfiguracao.PERMISSAO_VISUALIZADOR });
            Util.AdicionaInfoUsuarioNaTela(form1);
            Util.RegistraEntradaUsuario(this);
        }

    }
}