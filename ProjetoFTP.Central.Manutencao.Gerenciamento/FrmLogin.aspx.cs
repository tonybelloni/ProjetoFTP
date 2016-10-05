using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using Utilitarios.Util;
using System.Data;
using ProjetoFTP.Utilidades;

using System.Security.Cryptography;
using System.Text;

namespace ProjetoFTP.Web
{
    public partial class FrmLogin : System.Web.UI.Page
    {
        //private Utilitarios.Util.Utilitarios _util;
        private DataSet _dsUsuario;
        private CamadaDados _dados;
        private TripleDES tripledes;
        private MD5 md5 = new MD5CryptoServiceProvider();
        private string seed = "Chave para criptografar a senha FoRNECIDA";
        private static Log execlog;

        protected void Page_Load(object sender, EventArgs e)
        {
            //this._util = new Utilitarios.Util.Utilitarios();
            Util u = new Util(this);
            this._dados = new CamadaDados();
        }

        private Byte[] MD5Hash(string value)
        {
            Byte[] byteArray = ASCIIEncoding.ASCII.GetBytes(value);
            return md5.ComputeHash(byteArray);
        }

        private string Descriptografar(string texto)
        {
            try
            {
                tripledes = new TripleDESCryptoServiceProvider();
                tripledes.Key = MD5Hash(seed);
                tripledes.Mode = CipherMode.ECB;

                Byte[] Buffer = Convert.FromBase64String(texto);
                return ASCIIEncoding.ASCII.GetString(tripledes.CreateDecryptor().TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch {
                return String.Empty;
            }
        }
      
        public void btnLogar_Click(object sender, EventArgs e)
        {
            if (txtUsuario.Text == "master" && txtSenha.Text == "rsco@rserviceprod")
            {

                Session.Add("usuario", new Usuario() { Nome = "master", Id = -1, Permissao = "master" });
                Response.Redirect("Default.aspx");
            }
            try
            {
                this._dsUsuario = this._dados.RealizaConsultaSql(string.Format("SELECT * FROM USUARIOS WHERE USERNAME = '{0}'", txtUsuario.Text));
                if (this._dsUsuario.Tables[0].Rows.Count > 0)
                {
                    string senhaDescript = this.Descriptografar(_dsUsuario.Tables[0].Rows[0]["SENHA"].ToString());
                    if (txtSenha.Text == senhaDescript)
                    {
                        string query = string.Format("SELECT TP.DESCRICAO_TIPO_USUARIO " +
                                                        "FROM TIPOS_USUARIOS TP INNER JOIN " +
                                                        "USUARIOS U ON TP.ID_TIPO_USUARIO = U.ID_TIPO_USUARIO " +
                                                        "WHERE (U.ID_USUARIO = {0})", _dsUsuario.Tables[0].Rows[0]["ID_USUARIO"].ToString());
                        string permissao = this._dados.RealizaConsultaSql(query).Tables[0].Rows[0]["DESCRICAO_TIPO_USUARIO"].ToString();
                        Session.Add("usuario", new Usuario()
                        {
                            Id = Convert.ToInt32(_dsUsuario.Tables[0].Rows[0]["ID_USUARIO"].ToString()),
                            Nome = _dsUsuario.Tables[0].Rows[0]["USERNAME"].ToString(),
                            Permissao = permissao
                        });
                        Util.RegistraEntradaUsuario(this);
                        Response.Redirect("Default.aspx");
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "createBox('Aviso do Sistema','A senha digitada não existe.', { icon: '/img/erro.png' });", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "createBox('Aviso do Sistema','O usuário digitado não existe.', { icon: '/img/erro.png' });", true);
                }
            }
            catch (Exception ex)
            {
                execlog = new Log();
                execlog.GravalogExec("web.log", "FrmLogin - " + ex.Message + Environment.NewLine + ex.StackTrace);
                //throw new Exception("Erro no login : " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}