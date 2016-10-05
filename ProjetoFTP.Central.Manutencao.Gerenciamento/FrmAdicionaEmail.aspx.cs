using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProjetoFTP.Utilidades;
using System.Text.RegularExpressions;


namespace ProjetoFTP.Web
{
    public partial class FrmAdicionaEmail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Util.VerificaSessaoUsuario(this, "usuario");
            Util.VerificaPermissao(this, new string[] { CamadaConfiguracao.PERMISSAO_ADMINISTRADOR });
            Util.AdicionaInfoUsuarioNaTela(form1);
            Util.RegistraEntradaUsuario(this);
        }

        public void btnAdicionar_Click(object sender, EventArgs e)
        {
            try
            {
                AdicionaEmail();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "rsAlert('Aviso do Sistema','Email adicionado com sucesso!')", true);
            }
            catch(Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "rsAlert('Aviso do Sistema','" + ex.Message + "')", true);
            }
        }

        public void AdicionaEmail()
        {
            Regex regex = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
            if (regex.IsMatch(txtEmail.Text))
            {
                CamadaDados dados = new CamadaDados();
                string[] emails = dados.GetLinhasTxt(CamadaConfiguracao.CAMINHO_EMAILS);
                if (!emails.Contains(txtEmail.Text))
                {
                    dados.EscreveArquivoTxt(CamadaConfiguracao.CAMINHO_EMAILS, txtEmail.Text);
                }
                else
                {
                    throw new Exception("Email já existe.");
                }
            }
            else
            {
                throw new Exception("Email está no formato errado.");
            }
        }
    }
}