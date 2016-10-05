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
    public partial class FrmTerminais : System.Web.UI.Page
    {
        CamadaDados dados;

        protected void Page_Load(object sender, EventArgs e)
        {
            Util.VerificaSessaoUsuario(this, "usuario");
            Util.VerificaPermissao(this, new string[] { CamadaConfiguracao.PERMISSAO_ADMINISTRADOR });
            Util.AdicionaInfoUsuarioNaTela(page_bar);
            Util.RegistraEntradaUsuario(this);
            this.dados = new CamadaDados();
            if (!IsPostBack)
            {
                CarregaEstacoes();
            }
            lblPaginas.Text = string.Format("{0} de {1}", gvEstacoes.PageIndex + 1, gvEstacoes.PageCount);
            HabilitaDesabilitaBotoes();
        }

        public void btnProximo_Click(object sender, EventArgs e)
        {
            CarregaEstacoes();
            gvEstacoes.PageIndex = ++gvEstacoes.PageIndex;
            gvEstacoes.DataBind();
            lblPaginas.Text = string.Format("{0} de {1}", gvEstacoes.PageIndex + 1, gvEstacoes.PageCount);
            HabilitaDesabilitaBotoes();
        }

        public void btnAnterior_Click(object sender, EventArgs e)
        {
            CarregaEstacoes();
            gvEstacoes.PageIndex = --gvEstacoes.PageIndex;
            gvEstacoes.DataBind();
            lblPaginas.Text = string.Format("{0} de {1}", gvEstacoes.PageIndex + 1, gvEstacoes.PageCount);
            HabilitaDesabilitaBotoes();
        }

        public void btnAdicionar_Click(object sender, EventArgs e)
        {
            string query = string.Format("insert into ESTACOES(IP,NOME_MAQUINA ,HABILITADA) values('{0}','{1}',{2})",txtIp.Text, txtNomeMaquina.Text, ddlMaquinaAtiva.SelectedValue);
            dados.RealizaConsultaSql(query);
            Response.Redirect("/estacoes");
        }

        public void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                string query = string.Format("update ESTACOES set IP = '{0}',NOME_MAQUINA = '{1}', HABILITADA={2} where IP='{3}'", txtIpEditar.Text, txtNomeEditar.Text, ddlStatusEditar.SelectedValue, (string)Session["ip_editar"]);
                dados.RealizaConsultaSql(query);
            }
            catch
            {
            }
            Response.Redirect("/estacoes");
        }

        public void gvEstacoes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string value = Server.HtmlDecode(e.Row.Cells[e.Row.Cells.Count -1].Text);
                if (value == "0")
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Text = "<span class='red_box'>Desativada</span>";
                }
                else
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Text = "<span class='green_box'>Ativada</span>";
                }
            }

        }

        public void gvEstacoes_RowCommand(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Editar")
            {
                string ip = Server.HtmlDecode(gvEstacoes.Rows[Convert.ToInt32(e.CommandArgument)].Cells[2].Text);
                Session["ip_editar"] = ip;
                string query = string.Format("select * from ESTACOES where IP = '{0}'", ip);
                DataTable ds = dados.RealizaConsultaSql(query).Tables[0];
                if (ds.Rows.Count > 0)
                {
                    txtNomeEditar.Text = ds.Rows[0]["NOME_MAQUINA"].ToString();
                    txtIpEditar.Text = ds.Rows[0]["IP"].ToString();
                    ddlStatusEditar.SelectedValue = ds.Rows[0]["HABILITADA"].ToString();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "showHideBox('#edit_box')", true);
                }
            }
        }

        private void HabilitaDesabilitaBotoes()
        {
            if (gvEstacoes.PageIndex == 0)
            {
                btnAnterior.Enabled = false;
            }
            else
            {
                btnAnterior.Enabled = true;
            }
            if (gvEstacoes.PageIndex == gvEstacoes.PageCount - 1)
            {
                btnProximo.Enabled = false;
            }
            else
            {
                btnProximo.Enabled = true;
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