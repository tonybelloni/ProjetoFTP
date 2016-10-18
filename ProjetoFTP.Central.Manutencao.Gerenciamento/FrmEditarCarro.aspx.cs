using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProjetoFTP.Utilidades;
using System.Data;

namespace ProjetoFTP.Web
{
    public partial class FrmEditarCarro : System.Web.UI.Page
    {
        CamadaDados dados;
        protected void Page_Load(object sender, EventArgs e)
        {
            Util.VerificaSessaoUsuario(this, "usuario");
            Util.VerificaPermissao(this, new string[] { CamadaConfiguracao.PERMISSAO_ADMINISTRADOR });
            Util.RegistraEntradaUsuario(this);
            dados = new CamadaDados();
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null)
                {
                    try
                    {
                        DataTable ds = dados.RealizaConsultaSql(string.Format("SELECT * FROM CARROS WHERE ID_CARRO={0}", Request.QueryString["id"])).Tables[0];
                        if (ds.Rows.Count > 0)
                        {
                            txtNumero.Text = string.Format(Request.QueryString["id"].ToString());
                            txtIp.Text = ds.Rows[0]["IP"].ToString();
                            txtUsuario.Text = ds.Rows[0]["USUARIO"].ToString();
                            txtSenha.Text = ds.Rows[0]["SENHA"].ToString();
                            //txtVerificado.Text = ds.Rows[0]["VERIFICADO"].ToString();
                            //txtUltimaCopia.Text = ds.Rows[0]["ULTIMA_COPIA"].ToString();
                            //txtUltimaVerificacao.Text = ds.Rows[0]["ULTIMA_VERIFICACAO"].ToString();
                        }
                    }
                    catch
                    {
                        Response.Redirect("/carros");
                    }
                }
                else
                {
                    Response.Redirect("/carros");
                }
            }
        }

        public void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                string query = string.Format("UPDATE CARROS SET IP='{0}', USUARIO='{1}', SENHA='{2}' WHERE ID_CARRO={3}", new object[] { txtIp.Text, txtUsuario.Text, txtSenha.Text, Request.QueryString["id"] });
                dados.RealizaConsultaSql(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Response.Redirect("/carros");
            }
        }
    }
}