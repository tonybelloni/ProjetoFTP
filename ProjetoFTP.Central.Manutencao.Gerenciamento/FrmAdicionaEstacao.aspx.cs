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
    public partial class FrmAdicionaEstacao : System.Web.UI.Page
    {
        CamadaDados dados;
        protected void Page_Load(object sender, EventArgs e)
        {
            Util.VerificaSessaoUsuario(this, "usuario");
            Util.VerificaPermissao(this, new string[] { CamadaConfiguracao.PERMISSAO_ADMINISTRADOR });
            Util.AdicionaInfoUsuarioNaTela(form1);
            Util.RegistraEntradaUsuario(this);
            dados = new CamadaDados();
        }

        public void btnAdicionar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtIp.Text) || string.IsNullOrEmpty(txtNome.Text)) 
                    throw new Exception();
                string query = string.Format("INSERT INTO ESTACOES(IP,NOME_MAQUINA,PORTA,HABILITADA) values('{0}','{1}',9898,1)",new object[] { txtIp.Text, txtNome.Text});
                dados.RealizaConsultaSqlVoid(query);
                pnlMensagem.Visible = true;
                pnlMensagem.CssClass = "positiva";
                lblMensagem.Text = "Estação cadastrada com sucesso!<br> Agora ela já está apta a copiar arquivos.";
            }
            catch
            {
                pnlMensagem.Visible = true;
                pnlMensagem.CssClass = "negativa";
                lblMensagem.Text = "Erro ao cadastrar estação!<br>Verifique se o ip já está sendo utilizado por alguma outra estação e tente novamente.";
            }
        }
    }
}