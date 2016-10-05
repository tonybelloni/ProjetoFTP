using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ProjetoFTP.Utilidades;

namespace ProjetoFTP.Web
{
    public partial class FrmEditaEstacao : System.Web.UI.Page
    {
        CamadaDados dados;
        protected override void OnInit(EventArgs e)
        {
            if (Request.QueryString["ip"] != null)
            {
                Util.VerificaSessaoUsuario(this, "usuario");
                Util.VerificaPermissao(this, new string[] { CamadaConfiguracao.PERMISSAO_ADMINISTRADOR });
                Util.AdicionaInfoUsuarioNaTela(form1);
                Util.RegistraEntradaUsuario(this);
                string ip = Server.UrlDecode(Request.QueryString["ip"].ToString());
                dados = new CamadaDados();
                DataSet ds = dados.RealizaConsultaSql(string.Format("SELECT * FROM ESTACOES WHERE IP='{0}'",ip));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    lblEstacao.Text = ds.Tables[0].Rows[0]["IP"].ToString();
                    txtNome.Text = ds.Tables[0].Rows[0]["NOME_MAQUINA"].ToString();
                }
                else
                {
                    Response.Redirect("FrmEstacoes.aspx");
                }
            }
            else
            {
                Response.Redirect("FrmEstacoes.aspx");
            }
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = Request.QueryString["ip"].ToString();
                string nomeMaquina = txtNome.Text;
                dados.RealizaConsultaSqlVoid(string.Format("UPDATE ESTACOES SET NOME_MAQUINA = '{0}' WHERE IP='{1}'",nomeMaquina,ip));
                pnlMensagem.CssClass = "positiva";
                lblMensagem.Text = "Estação editada com sucesso!<br>A estação selecionada teve seu nome alterado com sucesso.";
                pnlMensagem.Visible = true;
            }
            catch
            {
                pnlMensagem.CssClass = "negativa";
                lblMensagem.Text = "Erro ao editar estação!<br>Tente novamente.";
                pnlMensagem.Visible = true;
            }
        }
    }
}