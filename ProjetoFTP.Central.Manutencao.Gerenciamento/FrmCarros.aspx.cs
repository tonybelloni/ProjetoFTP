using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProjetoFTP.Utilidades;
using System.Data;
using System.Net.NetworkInformation;

namespace ProjetoFTP.Web
{
    public partial class FrmCarros : System.Web.UI.Page
    {
        CamadaDados dados;

        protected void Page_Load(object sender, EventArgs e)
        {
            Util.VerificaSessaoUsuario(this, "usuario");
            Util.VerificaPermissao(this, new string[] { CamadaConfiguracao.PERMISSAO_ADMINISTRADOR });
            Util.AdicionaInfoUsuarioNaTela(form1);
            Util.RegistraEntradaUsuario(this);
            dados = new CamadaDados();
            CarregaCarros();
        }

        public void btnProcurar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtProcuraCarro.Text))
                {
                    DataSet ds = dados.RealizaConsultaSql("SELECT * FROM CARROS WHERE ID_CARRO =" + txtProcuraCarro.Text + " ORDER BY IP DESC");
                    gvCarros.DataSource = ds;
                    gvCarros.DataBind();
                }
                else
                {
                    CarregaCarros();
                }
            }
            catch
            {
                CarregaCarros();
            }
        }

        public void gvCarros_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCarros.PageIndex = e.NewPageIndex;
            gvCarros.DataBind();
            System.Threading.Thread.Sleep(1000);
        }

        private void CarregaCarros()
        {
            DataSet ds = dados.RealizaConsultaSql("SELECT * FROM CARROS order by IP");
            gvCarros.DataSource = ds;
            gvCarros.DataBind();
        }

        protected void gvCarros_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                Response.Redirect("FrmEditaCarro.aspx?id=" + gvCarros.Rows[Convert.ToInt32(e.CommandArgument)].Cells[2].Text);
            }
            else if (e.CommandName == "Copiar")
            {
                int idCarro = Convert.ToInt32(Server.HtmlDecode(gvCarros.Rows[Convert.ToInt32(e.CommandArgument)].Cells[2].Text));
                ForcaCopia(idCarro);
            }
        }

        public void gvCarros_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string ipCarro = Server.HtmlDecode(e.Row.Cells[3].Text).Trim();
                Button btnForcaCopia = (Button)e.Row.Cells[1].Controls[0];
                if (string.IsNullOrEmpty(ipCarro))
                {
                    btnForcaCopia.Enabled = false;
                    btnForcaCopia.CssClass = "btn disable";
                }
                else
                {
                    btnForcaCopia.CssClass = "btn";
                }
                e.Row.Cells[5].Text = "***";
                e.Row.Cells[8].Text = (Server.HtmlDecode(e.Row.Cells[8].Text) == "1") ? "Sim" : "Não";
            }
        }

        public void ForcaCopia(int carro)
        {
            try
            {
                new CamadaControle().ForcaCopia(carro);
                CodigoSistema cod = CamadaConfiguracao.COD_38301;
                Util.RegistraEventoUsuario(this,Convert.ToInt32(cod.Id), cod.Mensagem  + " - " + carro);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "rsAlert('Aviso do Sistema','O carro será copiado em breve.');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "rsAlert('Aviso do Sistema','Não foi possível forçar cópia do carro.<br>" + ex.Message + "');", true);
            }
        }
    }
}