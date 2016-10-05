using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ProjetoFTP.Utilidades;
using System.Net.NetworkInformation;

namespace ProjetoFTP.Web
{
    public partial class FrmEstacoes : System.Web.UI.Page
    {
        CamadaDados dados;

        protected override void OnInit(EventArgs e)
        {

            Util.VerificaSessaoUsuario(this, "usuario");
            Util.VerificaPermissao(this, new string[] { CamadaConfiguracao.PERMISSAO_ADMINISTRADOR });
            Util.AdicionaInfoUsuarioNaTela(form1);
            Util.RegistraEntradaUsuario(this);
            dados = new CamadaDados();
            CarregaEstacoes(); 
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["d"] == "t")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "rsAlert('Aviso do Sistema','a estação foi deletada com sucesso.');", true);
            }
            else if (Request.QueryString["d"] == "f")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "rsAlert('Aviso do Sistema','erro ao deletar a estação.');", true);
            }
        }

        public void btnPesquisar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPesquisar.Text))
            {
                DataSet ds = dados.RealizaConsultaSql("SELECT * FROM ESTACOES WHERE NOME_MAQUINA LIKE '%" + txtPesquisar.Text + "%'");
                gvEstacoes.DataSource = ds;
                gvEstacoes.DataBind();
            }
            else
            {
                CarregaEstacoes();
            }
        }

        public void gvEstacoes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                string ipEncode = Server.UrlEncode(Server.HtmlDecode(gvEstacoes.Rows[Convert.ToInt32(e.CommandArgument)].Cells[5].Text));
                Response.Redirect("FrmEditaEstacao.aspx?ip=" + ipEncode);
            }
            else if (e.CommandName == "Delete")
            {
                try
                {
                    string ip = Server.HtmlDecode(gvEstacoes.Rows[Convert.ToInt32(e.CommandArgument)].Cells[5].Text);
                    dados.RealizaConsultaSqlVoid(string.Format("DELETE ESTACOES WHERE IP = '{0}'", ip));
                }
                finally
                {
                    Response.Redirect("FrmEstacoes.aspx");
                }
            }
            else if (e.CommandName == "Habilitar")
            {
                string ip = Server.HtmlDecode(gvEstacoes.Rows[Convert.ToInt32(e.CommandArgument)].Cells[5].Text);
                if (Convert.ToInt32(dados.RealizaConsultaSql(string.Format("SELECT HABILITADA FROM ESTACOES WHERE IP='{0}'", ip)).Tables[0].Rows[0][0]) == 0)
                {
                    //ativa
                    dados.RealizaConsultaSqlVoid(string.Format("UPDATE ESTACOES SET HABILITADA = 1 WHERE IP='{0}'", ip));
                    CarregaEstacoes();

                }
                else
                {
                    //inativa
                    dados.RealizaConsultaSqlVoid(string.Format("UPDATE ESTACOES SET HABILITADA = 0 WHERE IP='{0}'", ip));
                    CarregaEstacoes();
                }
            }
            else if (e.CommandName == "Testar")
            {
                Ping png = new Ping();
                PingReply pr;
                try
                {
                    string ip = Server.HtmlDecode(gvEstacoes.Rows[Convert.ToInt32(e.CommandArgument)].Cells[4].Text);
                    pr = png.Send(ip);
                    if (pr.Status == IPStatus.Success)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "rsAlert('Aviso do Sistema','a estação está respondendo.');", true);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "rsAlert('Aviso do Sistema','a estação não está respondendo.');", true);
                }
            }
            updEstacoes.Update();
        }

        public void gvEstacoes_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Panel panel = (Panel)e.Row.FindControl("pnlAtiva");
                Label label = (Label)e.Row.FindControl("lblAtiva");
                string result = null;
                if (label.Text == "0")
                {
                    panel.CssClass = "negativa";
                    result = "Desativada";
                }
                else
                {
                    panel.CssClass = "positiva";
                    result = "Ativada";
                }
                label.Text = result;
            }
        }

        public void CarregaEstacoes()
        {
            DataSet ds = dados.RealizaConsultaSql("SELECT * FROM ESTACOES");
            gvEstacoes.DataSource = ds;
            gvEstacoes.DataBind();
        }
    }
}