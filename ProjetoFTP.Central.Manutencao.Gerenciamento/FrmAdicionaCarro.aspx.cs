using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProjetoFTP.Utilidades;

namespace ProjetoFTP.Web
{
    public partial class FrmAdicionaCarro : System.Web.UI.Page
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
                if (txtSenha.Text != txtSenha2.Text)
                    throw new Exception("");
                string ip = txtIp.Text;
                string numero = txtNumero.Text;
                string usuario = txtUsuario.Text;
                string senha = txtSenha.Text;
                string query = string.Format("insert into CARROS       " + 
                                             "(ID_CARRO,               " +
                                             " IP,                     " +
                                             " USUARIO,                " + 
                                             " SENHA,                  " + 
                                             " VERIFICADO,             " +
                                             " ULTIMA_COPIA,           " + 
                                             " ULTIMO_ARQUIVO_COPIADO, " +
                                             " FLAG_ULTIMA_COPIA,      " +
                                             " ULTIMA_VERIFICACAO,     " +
                                             " QTDE_CAMERAS,           " +
                                             " COPIANDO,               " +
                                             " EMPRESA_NUM)            " +
                                             "values                   " +
                                             "({0},                    " + 
                                             "'{1}',                   " +
                                             "'{2}',                   " +
                                             "'{3}',                   " + 
                                             "0,                       " +
                                             "sysdate-1,               " +
                                             "null,                    " +
                                             "0,                       " +
                                             "sysdate-1,               " +
                                             "4,                       " +
                                             "0,                       " +
                                             "null)", new object[] { numero, ip, usuario, senha });

                dados.RealizaConsultaSqlVoid(query);

                pnlMensagem.Visible = true;
                lblMensagem.Text = "Carro cadastrado com sucesso!";
                pnlMensagem.CssClass = "positiva";
            }
            catch
            {
                pnlMensagem.Visible = true;
                lblMensagem.Text = "Algo deu errado!<br>Verifique se o numero do carro e o ip já não está sendo usado por algum carro.";
                pnlMensagem.CssClass = "negativa";
            }
        }
    }
}