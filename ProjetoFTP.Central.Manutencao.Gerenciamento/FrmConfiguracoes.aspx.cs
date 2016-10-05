using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProjetoFTP.Utilidades;
using System.Data;
using System.Web.UI.HtmlControls;

namespace ProjetoFTP.Web
{
    public partial class FrmConfiguracoes : System.Web.UI.Page
    {
        private CamadaDados _dados;

        protected override void OnInit(EventArgs e)
        {
            Util.VerificaSessaoUsuario(this, "usuario");
            Util.VerificaPermissao(this, new string[] { CamadaConfiguracao.PERMISSAO_ADMINISTRADOR });
            Util.AdicionaInfoUsuarioNaTela(form1);
            Util.RegistraEntradaUsuario(this);
            this._dados = new CamadaDados();
            DataSet ds = this._dados.RealizaConsultaSql("SELECT * FROM PARAMETROS_SISTEMAS");
            txtSleep.Text = ds.Tables[0].Rows[0]["SLEEP_VERIFICACAO"].ToString();
            txtPercentAceitavel.Text = ds.Tables[0].Rows[0]["PERCENT_IMAGENS_ACEITAVEL"].ToString();
            txtPercentPreocupante.Text = ds.Tables[0].Rows[0]["PERCENT_IMAGENS_CRITICO"].ToString();
            base.OnInit(e);
        }

        protected override void RenderChildren(HtmlTextWriter writer)
        {
            base.RenderChildren(writer);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                this._dados.RealizaConsultaSqlVoid(string.Format("UPDATE PARAMETROS_SISTEMAS SET SLEEP_VERIFICACAO = {0}, PERCENT_IMAGENS_ACEITAVEL = {1}, PERCENT_IMAGENS_CRITICO = {2}",new object[]{ txtSleep.Text, txtPercentAceitavel.Text, txtPercentPreocupante.Text }));
                ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "rsAlert('Aviso do Sistema','Alterações salvas com sucesso.');", true);
            }
            catch(Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "rsAlert('Aviso do Sistema','Erro ao salvar alterações.<br>Verifique se os campos contém somente números.');", true);
            }
        }
    }
}