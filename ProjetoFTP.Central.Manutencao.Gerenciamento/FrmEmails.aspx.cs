using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProjetoFTP.Utilidades;
using System.IO;
namespace ProjetoFTP.Web
{
    public partial class FrmEmails : System.Web.UI.Page
    {
        private CamadaControle _camadaControle;
        private List<Email> _emails;
        protected void Page_Load(object sender, EventArgs e)
        {
            Util.VerificaSessaoUsuario(this, "usuario");
            Util.VerificaPermissao(this, new string[] { CamadaConfiguracao.PERMISSAO_ADMINISTRADOR });
            Util.AdicionaInfoUsuarioNaTela(form1);
            Util.RegistraEntradaUsuario(this);

            this._camadaControle = new CamadaControle();
            this._emails = CarregaEmails();
            gvEmails.DataSource = this._emails;
            gvEmails.DataBind();
        }

        public void gvEmails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Remover")
            {
                string email = Server.HtmlDecode(gvEmails.Rows[Convert.ToInt32(e.CommandArgument)].Cells[1].Text).Trim();
                new CamadaDados().RemoveLinhaTxt(CamadaConfiguracao.CAMINHO_EMAILS,email);
                this._emails = CarregaEmails();
                gvEmails.DataSource = this._emails;
                gvEmails.DataBind();
            }
        }

        public void btnProcurar_Click(object sender, EventArgs e)
        {
            gvEmails.DataSource = FiltraEmails(txtProcurar.Text);
            gvEmails.DataBind();
        }

        public List<Email> CarregaEmails()
        {
            string[] emailsList = File.ReadAllLines(CamadaConfiguracao.CAMINHO_EMAILS);
            List<Email> emails = new List<Email>();
            for (int i = 0; i < emailsList.Length; i++)
            {
                Email email = new Email() { Endereco = emailsList[i] };
                emails.Add(email);
            }
            return emails;
        }

        public List<Email> FiltraEmails(string email)
        {
            return this._emails.Where(e => e.Endereco.Contains(email)).ToList();
        }
    }
}