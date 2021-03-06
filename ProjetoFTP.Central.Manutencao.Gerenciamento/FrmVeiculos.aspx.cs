﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProjetoFTP.Utilidades;
using System.Data;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace ProjetoFTP.Web
{
    public partial class FrmVeiculos : System.Web.UI.Page
    {
        CamadaDados dados;

        protected void Page_Load(object sender, EventArgs e)
        {
            Util.RegistraEntradaUsuario(this);
            Util.VerificaSessaoUsuario(this, "usuario");
            Util.AdicionaInfoUsuarioNaTela(page_bar);
            dados = new CamadaDados();
            if (!IsPostBack)
            {
                CarregaCarros();
            }
            lblPaginas.Text = string.Format("{0} de {1}", gvCarros.PageIndex + 1, gvCarros.PageCount);
            HabilitaDesabilitaBotoes();
        }

        private void CarregaCarros()
        {
            DataSet ds = dados.RealizaConsultaSql("SELECT * FROM CARROS order by IP");
            gvCarros.DataSource = ds;
            gvCarros.DataBind();
        }

        public void btnCancelarAddCarro_Click(object sender, EventArgs e)
        {
            Response.Redirect("/carros");
        }

        public void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                string query = string.Format("UPDATE CARROS SET IP='{0}', USUARIO='{1}', SENHA='{2}' WHERE ID_CARRO={3}", new object[] { txtAltIp.Text, txtAltUsuario.Text, txtAltSenha.Text, txtAltNumero.Text});
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

        public void btnAdicionarCarro_Click(object sender, EventArgs e)
        {
            try
            {
                string pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
                Regex check = new Regex(pattern);

                if (string.IsNullOrEmpty(txtNumeroCarroAdd.Text))
                    throw new Exception("O número do carro deve ser informado");

                if (!check.IsMatch(txtIp.Text) || string.IsNullOrEmpty(txtIp.Text))
                    throw new Exception("IP digitado não é válido ou não foi informado");

                if (string.IsNullOrEmpty(txtUsuario.Text))
                    throw new Exception("Nome do usuário do FTP é obrigatório");

                if (string.IsNullOrEmpty(txtSenha.Text))
                    throw new Exception("Senha do usuário do FTP é obrigatória");

                if ( (txtSenha.Text != txtRepetirSenha.Text) || string.IsNullOrEmpty(txtSenha.Text) )
                    throw new Exception("O campo de validação de senha não é igual a primeira senha digitada");

                string ultimo_arquivo;

                ultimo_arquivo = string.Format("/imagens/002/{0}/{1}/event{2}002.avi", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HH"), DateTime.Now.ToString("yyyyMMddHHmmss"));
 
                //this.dados.RealizaConsultaSql(string.Format("INSERT INTO CARROS(ID_CARRO, IP, USUARIO, SENHA) values({0},'{1}','{2}','{3}')", new object[] { id_carro, ip, usuario, senha }));
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
                                             " EMPRESA_NUM,            " + 
                                             " ATIVO)                  " +
                                             "values                   " +
                                             "({0},                    " +
                                             "'{1}',                   " +
                                             "'{2}',                   " +
                                             "'{3}',                   " +
                                             "0,                       " +
                                             "sysdate-1,               " +
                                             "'{4}',                   " +
                                             "0,                       " +
                                             "sysdate-1,               " +
                                             "4,                       " +
                                             "0,                       " +
                                             "null,                    " +
                                             "1                       )", new object[] { txtNumeroCarroAdd.Text, txtIp.Text, txtUsuario.Text, txtSenha.Text, ultimo_arquivo});

                this.dados.RealizaConsultaSql(query);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "showAlert('#aviso','Aviso do Sistema','Carro adicionado com sucesso','good_thing');", true);
            }
            catch(Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "showAlert('#aviso','Aviso do Sistema','" + ex.Message + "','bad_thing');", true);
            }
        }

        public void btnBuscarCarro_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNumeroCarro.Text) && txtNumeroCarro.Text != "Número do carro")
            {
                DataSet ds = dados.RealizaConsultaSql("SELECT * FROM CARROS where ID_CARRO = " + txtNumeroCarro.Text);
                gvCarros.DataSource = ds;
                gvCarros.DataBind();
            }
            else
            {
                CarregaCarros();
            }
        }

        public void gvCarros_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Copiar")
            {
                try
                {
                    int idCarro = Convert.ToInt32(Server.HtmlDecode(gvCarros.Rows[Convert.ToInt32(e.CommandArgument)].Cells[2].Text));
                    DataSet ds = dados.RealizaConsultaSql(string.Format("select * from carros where id_carro={0}", idCarro));
                    string ip = ds.Tables[0].Rows[0]["IP"].ToString();
                    if (string.IsNullOrEmpty(ip))
                        throw new Exception("O carro selecionado não teve seu ip cadastrado");
                    string usuario = ds.Tables[0].Rows[0]["USUARIO"].ToString();
                    string senha = ds.Tables[0].Rows[0]["SENHA"].ToString();
                    PingaIp(ip);
                    TestaConexaoFtp(ip, usuario, senha);
                    new CamadaControle().ForcaCopia(idCarro);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "createBox('Aviso do Sistema','Carro enviado para cópia com sucesso.<br><b>Aguarde o início da cópia.</b>', { width : 'auto' , height : 'auto', icon : '/img/certo.png' });", true);
                    CodigoSistema cod = CamadaConfiguracao.COD_38301;
                    Util.RegistraEventoUsuario(this, Convert.ToInt32(cod.Id), cod.Mensagem + " - " + idCarro);
                    ((Button)gvCarros.Rows[Convert.ToInt32(e.CommandArgument)].Cells[1].Controls[0]).Enabled = false;
                }
                catch (Exception ex)
                {
                    string erro = ex.Message;
                    string procedimentos = String.Empty;
                    if (ex.Message == "O carro não está comunicando.")
                    {
                        procedimentos = "<div>" + 
                                            "<b>Procedimentos: </b>" +
                                            "<ul>" +
                                                "<li><b>Procedimento 1: </b> Verificar se o carro está na garagem.</li>" +
                                                "<li><b>Procedimento 2: </b> Verificar se o DVR está conectado.</li>" +
                                                "<li><b>Procedimento 3: </b> Verificar se o DVR está ligado.</li>" +
                                                "<li><b>Procedimento 4: </b> Verificar se o DVR não está travado.</li>" + 
                                            "</ul>" + 
                                        "</div>";
                    }
                    else if (ex.Message == "O carro está na garagem, mas não é possível copiar os arquivos.")
                    {
                        procedimentos = "<div>" +
                                            "<b>Procedimentos: </b>" +
                                            "<ul>" +
                                                "<li><b>Procedimento 1: </b> Verificar se o DVR tem o FTP habilitado.</li>" +
                                                "<li><b>Procedimento 2: </b> Verificar manualmente a conexão FTP.</li>" +
                                            "</ul>" +
                                        "</div>";
                    }
                    erro += procedimentos;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "createBox('Aviso do Sistema','" + erro + "', { width : 'auto' , height : 'auto' });", true);
                }
            }
            else if (e.CommandName == "Editar")
            {
                int idCarro = Convert.ToInt32(Server.HtmlDecode(gvCarros.Rows[Convert.ToInt32(e.CommandArgument)].Cells[2].Text));
                string query = string.Format("select * from carros where id_carro = {0}", idCarro);
                DataTable ds = dados.RealizaConsultaSql(query).Tables[0];
                if (ds.Rows.Count > 0)
                {
                    txtAltNumero.Text = ds.Rows[0]["ID_CARRO"].ToString();
                    txtAltIp.Text = ds.Rows[0]["IP"].ToString();
                    txtAltUsuario.Text = ds.Rows[0]["USUARIO"].ToString();
                    txtAltSenha.Text = ds.Rows[0]["SENHA"].ToString();
                    txtAltRepetirSenha.Text = ds.Rows[0]["SENHA"].ToString();
                    ddlStatusEditar.SelectedValue = ds.Rows[0]["ATIVO"].ToString();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "showHideBox('#edit_box')", true);
                }
            }
        }

        public void btnProximo_Click(object sender, EventArgs e)
        {
            CarregaCarros();
            gvCarros.PageIndex = ++gvCarros.PageIndex;
            gvCarros.DataBind();
            lblPaginas.Text = string.Format("{0} de {1}", gvCarros.PageIndex + 1, gvCarros.PageCount);
            HabilitaDesabilitaBotoes();
        }

        public void btnAnterior_Click(object sender, EventArgs e)
        {
            CarregaCarros();
            gvCarros.PageIndex = --gvCarros.PageIndex;
            gvCarros.DataBind();
            lblPaginas.Text = string.Format("{0} de {1}", gvCarros.PageIndex + 1, gvCarros.PageCount);
            HabilitaDesabilitaBotoes();
        }

        private void HabilitaDesabilitaBotoes()
        {
            if (gvCarros.PageIndex == 0)
            {
                btnAnterior.Enabled = false;
            }
            else
            {
                btnAnterior.Enabled = true;
            }
            if (gvCarros.PageIndex == gvCarros.PageCount - 1)
            {
                btnProximo.Enabled = false;
            }
            else
            {
                btnProximo.Enabled = true;
            }
        }

        public static void PingaIp(string ip)
        {
            Ping png = new Ping();
            PingReply pr;
            pr = png.Send(ip);
            if (pr.Status != IPStatus.Success)
                throw new Exception("O carro não está comunicando.");
        }

        public static void TestaConexaoFtp(string ip, string usuario, string senha)
        {
            FTP ftp = new FTP(ip, usuario, senha);
            if (!ftp.IsConnecting())
                throw new Exception("O carro está na garagem, mas não é possível copiar os arquivos.");
        }

        public void gvCarros_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string value = Server.HtmlDecode(e.Row.Cells[e.Row.Cells.Count - 1].Text);
                if (value == "0")
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Text = "<span class='red_box'>Inativo</span>";
                }
                else
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Text = "<span class='green_box'>Ativo</span>";
                }
            }

        }

    }
}