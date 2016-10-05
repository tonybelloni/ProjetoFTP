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
    public partial class FrmEditaCarro : System.Web.UI.Page
    {
        CamadaDados dados;

        protected override void OnInit(EventArgs e)
        {

            Util.VerificaSessaoUsuario(this, "usuario");
            Util.VerificaPermissao(this, new string[] { CamadaConfiguracao.PERMISSAO_ADMINISTRADOR });
            Util.AdicionaInfoUsuarioNaTela(form1);
            Util.RegistraEntradaUsuario(this);
            if (Request.QueryString["id"] != null)
            {
                dados = new CamadaDados();
                try
                {
                    DataTable ds = dados.RealizaConsultaSql(string.Format("SELECT * FROM CARROS WHERE ID_CARRO={0}", Request.QueryString["id"])).Tables[0];
                    if (ds.Rows.Count > 0)
                    {
                        lblCarro.Text = string.Format("Carro {0}", Request.QueryString["id"].ToString());
                        txtIp.Text = ds.Rows[0]["IP"].ToString();
                        txtUsuario.Text = ds.Rows[0]["USUARIO"].ToString();
                        txtSenha.Text = ds.Rows[0]["SENHA"].ToString();
                        txtVerificado.Text = ds.Rows[0]["VERIFICADO"].ToString();
                        txtUltimaCopia.Text = ds.Rows[0]["ULTIMA_COPIA"].ToString();
                        txtUltimaVerificacao.Text = ds.Rows[0]["ULTIMA_VERIFICACAO"].ToString();
                    }
                    else
                    {
                        Response.Redirect("FrmCarros.aspx");
                    }
                }
                catch
                {
                    Response.Redirect("FrmCarros.aspx");
                }
            }
            else
            {
                Response.Redirect("FrmCarros.aspx");
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
                string query = string.Format("UPDATE CARROS SET IP='{0}', USUARIO='{1}', SENHA='{2}', VERIFICADO={3}, ULTIMA_COPIA=to_date('{4}','dd/mm/yyyy hh24:mi:ss'), ULTIMA_VERIFICACAO=to_date('{5}','dd/mm/yyyy hh24:mi:ss') WHERE ID_CARRO={6}", new object[] { txtIp.Text, txtUsuario.Text, txtSenha.Text, txtVerificado.Text,txtUltimaCopia.Text, txtUltimaVerificacao.Text, Request.QueryString["id"] });
                dados.RealizaConsultaSql(query);
                pnlMensagem.Visible = true;
                lblMensagem.Text = "Alteração realizada!<br>O carro teve todas as suas alterações realizadas com sucesso.";
                pnlMensagem.CssClass = "positiva";
            }
            catch
            {
                pnlMensagem.Visible = true;
                lblMensagem.Text = "Algo deu errado!<br>Verifique se os dados foram preenchidos corretamente.";
                pnlMensagem.CssClass = "negativa";
            }
        }
    }
}