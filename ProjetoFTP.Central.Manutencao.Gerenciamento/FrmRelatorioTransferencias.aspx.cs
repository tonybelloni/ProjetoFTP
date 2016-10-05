using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ProjetoFTP.Utilidades;
using System.IO;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Net.NetworkInformation;
using ProjetoFTP.Utilidades;
using ProjetoFTP.Utilidades.Entidades;

namespace ProjetoFTP.Web
{
    public partial class FrmRelatorioTransferencias : System.Web.UI.Page
    {
        private List<RelatorioCopiaItem> _relatorio;
        private List<Email> _emails;
        private CamadaDados _camadaDados;

        protected void Page_Load(object sender, EventArgs e)
        {
            Util.RegistraEntradaUsuario(this);
            Util.VerificaSessaoUsuario(this, "usuario");
            Util.AdicionaInfoUsuarioNaTela(page_bar);
            this._camadaDados = new CamadaDados();
            this._emails = CarregaEmails();
            gvEmails.DataSource = this._emails;
            gvEmails.DataBind();
            this._relatorio = new List<RelatorioCopiaItem>();
            CarregaRelatorio();
            gvRelatorio.DataSource = FiltrarRelatorio();
            gvRelatorio.DataBind();
            btnFiltrar.Click += new EventHandler(btnFiltrar_Click);
            lblPaginas.Text = string.Format("{0} de {1}", gvRelatorio.PageIndex + 1, gvRelatorio.PageCount);
            HabilitaDesabilitaBotoes();
            CarregaEstacoes();
        }

        public static void PingaIp(string ip)
        {
            Ping png = new Ping();
            PingReply pr;
            pr = png.Send(ip);
            if (pr.Status != IPStatus.Success)
                throw new Exception("O carro não está comunicando. <br><ul class='lista_procedimentos'><li><b>Procedimentos:</b></li></ul>");
        }

        public static void TestaConexaoFtp(string ip, string usuario, string senha)
        {
            FTP ftp = new FTP(ip, usuario, senha);
            if (!ftp.IsConnecting())
                throw new Exception("O carro está na garagem, mas não é possível copiar os arquivos.");
        }

        public void CarregaRelatorio()
        {
            DataTable dt = this._camadaDados.RealizaConsultaSql("SELECT E.NOME_MAQUINA, CH.* " + 
                                                                "FROM COPIAS_HISTORICOS CH LEFT OUTER JOIN " + 
                                                                "ESTACOES E ON CH.IP_ESTACAO = E.IP " + 
                                                                /*"WHERE (CH.DATA_INICIO_COPIA > SYSDATE - 365) " +*/
                                                                "ORDER BY CH.DATA_INICIO_COPIA DESC").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                RelatorioCopiaItem item = new RelatorioCopiaItem()
                {
                    Id = Convert.ToInt32(dt.Rows[i]["ID_COPIA"]),
                    DataInicial = DateTime.Parse(dt.Rows[i]["DATA_INICIO_COPIA"].ToString()),
                    DataFinal = DateTime.Parse(dt.Rows[i]["DATA_FIM_COPIA"].ToString()),
                    NumeroCarro = Convert.ToInt32(dt.Rows[i]["ID_CARRO"].ToString()),
                    PenDrive = dt.Rows[i]["COD_EQUIPAMENTO"].ToString(),
                    QuantidadeArquivosValidos = (dt.Rows[i]["NUMERO_ARQUIVOS_VALIDOS"] == DBNull.Value) ? 0 : Convert.ToInt32(dt.Rows[i]["NUMERO_ARQUIVOS_VALIDOS"]),
                    QuantidadeArquivosCopiados = Convert.ToInt32(dt.Rows[i]["NUMERO_ARQUIVOS_COPIADOS"].ToString()),
                    QuantidadeArquivosTotal = Convert.ToInt32(dt.Rows[i]["NUMERO_ARQUIVOS_TOTAL"].ToString()),
                    VolumeArquivosCopiados = Convert.ToInt64(dt.Rows[i]["TAMANHO_ARQUIVOS_COPIADOS"].ToString()),
                    VolumeArquivosTotal = Convert.ToInt64(dt.Rows[i]["TAMANHO_ARQUIVOS_TOTAL"].ToString()),
                    PeriodoInicial = (dt.Rows[i]["PERIODO_INICIAL"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(dt.Rows[i]["PERIODO_INICIAL"].ToString()),
                    PeriodoFinal = (dt.Rows[i]["PERIODO_FINAL"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(dt.Rows[i]["PERIODO_FINAL"].ToString()),
                    TipoCopia = (Convert.ToInt32(dt.Rows[i]["TIPO_COPIA"].ToString()) == -1) ? "Guia" : ((Convert.ToInt32(dt.Rows[i]["TIPO_COPIA"].ToString()) == 0) ? "Ult Arq" : "Completa"),
                    Codigo = (dt.Rows[i]["CODIGO_RESULTADO"] == DBNull.Value) ? "N/A" : dt.Rows[i]["CODIGO_RESULTADO"].ToString(),
                    Usuario = "Busvision Gateway Server",
                    Estacao = (dt.Rows[i]["NOME_MAQUINA"] == DBNull.Value) ? "" : dt.Rows[i]["NOME_MAQUINA"].ToString()
                };
                this._relatorio.Add(item);
            }
        }

        public void CarregaEstacoes()
        {
            //carrega estacaoes que ja descarregaram alguma vez
            DataSet ds = this._camadaDados.RealizaConsultaSql("SELECT * FROM ESTACOES");
            ddlEstacao.DataSource = ds;
            ddlEstacao.DataTextField = "NOME_MAQUINA";
            ddlEstacao.DataValueField = "IP";
            ddlEstacao.DataBind();
            ddlEstacao.Items.Add(new ListItem("Todas","0"));
            ddlEstacao.Items[ddlEstacao.Items.Count - 1].Selected = true;
        }

        private void HabilitaDesabilitaBotoes()
        {
            if (gvRelatorio.PageIndex == 0)
            {
                btnAnterior.Enabled = false;
            }
            else
            {
                btnAnterior.Enabled = true;
            }
            if (gvRelatorio.PageIndex == gvRelatorio.PageCount - 1)
            {
                btnProximo.Enabled = false;
            }
            else
            {
                btnProximo.Enabled = true;
            }
        }

        public List<Email> CarregaEmails()
        {
            string[] emailsList = File.ReadAllLines(CamadaConfiguracao.CAMINHO_EMAILS);
            List<Email> emails = new List<Email>();
            for (int i = 0; i < emailsList.Length; i++)
            {
                Email email = new Email() { Endereco = emailsList[i] };
                emails.Add(email);
            }
            return emails;
        }

        public static void EnviarEmail(string email, GridView grid)
        {
            try
            {

                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                grid.AllowPaging = false;
                grid.DataBind();
                grid.RenderControl(hw);

                grid.AllowPaging = true;
                grid.DataBind();

                MailMessage mensagem = new MailMessage();
                mensagem.From = new MailAddress(CamadaConfiguracao.EMAIL_MALA_DIRETA_ENDERECO, "Busvision Gateway - Relatório de cópias");
                mensagem.To.Add(email);
                mensagem.IsBodyHtml = true;
                mensagem.Subject = "Busvision Gateway - Relatório de cópias:" + DateTime.Now.ToString("dd/MM/yyyy");
                mensagem.Body = sb.ToString();
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;

                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(CamadaConfiguracao.EMAIL_MALA_DIRETA_ENDERECO, CamadaConfiguracao.EMAIL_MALA_DIRETA_SENHA);
                smtp.Send(mensagem);
            }
            catch (Exception ex)
            {

            }
        }

        protected void gvRelatorios_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType != DataControlRowType.Header && e.Row.RowType != DataControlRowType.Footer && e.Row.RowType != DataControlRowType.Pager)
            {
                if (Convert.ToInt32(e.Row.Cells[7].Text) == 0)
                {
                    //vermelho
                    if ((e.Row.RowIndex) % 2 != 0)
                    {
                        //escuro
                        e.Row.BackColor = System.Drawing.Color.FromName("#FFA8A8");
                    }
                    else
                    {
                        //claro
                        e.Row.BackColor = System.Drawing.Color.FromName("#FFC1C1");
                    }

                }
                else
                {
                    if (Convert.ToInt32(e.Row.Cells[8].Text) < Convert.ToInt32(e.Row.Cells[7].Text) * 0.75)
                    {
                        //amarelo
                        if ((e.Row.RowIndex) % 2 != 0)
                        {
                            //escuro
                            e.Row.BackColor = System.Drawing.Color.FromName("#FFED84");
                        }
                        else
                        {
                            //claro
                            e.Row.BackColor = System.Drawing.Color.FromName("#FFFFCC");
                        }
                    }
                    else
                    {
                        //verde
                        if ((e.Row.RowIndex) % 2 != 0)
                        {
                            //escuro
                            e.Row.BackColor = System.Drawing.Color.FromName("#b8d47f");
                        }
                        else
                        {
                            //claro
                            e.Row.BackColor = System.Drawing.Color.FromName("#cde69c");
                        }
                    }
                }
                string c5 = Server.HtmlDecode(e.Row.Cells[2].Text).Trim();
                string c6 = Server.HtmlDecode(e.Row.Cells[3].Text).Trim();
                string c7 = Server.HtmlDecode(e.Row.Cells[4].Text).Trim();
                string c8 = Server.HtmlDecode(e.Row.Cells[5].Text).Trim();
                string c9 = Server.HtmlDecode(e.Row.Cells[6].Text).Trim();
                string c10 = Server.HtmlDecode(e.Row.Cells[7].Text).Trim();
                string c11 = Server.HtmlDecode(e.Row.Cells[8].Text).Trim();
                string c12 = Server.HtmlDecode(e.Row.Cells[9].Text).Trim();
                string c13 = Server.HtmlDecode(e.Row.Cells[10].Text).Trim();
                string c14 = Server.HtmlDecode(e.Row.Cells[11].Text).Trim();
                string c15 = Server.HtmlDecode(e.Row.Cells[12].Text).Trim();
                if (string.IsNullOrEmpty(c6))
                {
                    e.Row.Cells[4].Text = "-";
                }
                if (c7.Contains("0,0 min"))
                {
                    e.Row.Cells[5].Text = "-";
                    e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Center;
                }
                if (c11.Contains("0,0 MB"))
                {
                    e.Row.Cells[9].Text = "-";
                    e.Row.Cells[9].HorizontalAlign = HorizontalAlign.Center;
                }
                if (c11.Contains("0,0 MB"))
                {
                    e.Row.Cells[9].Text = "-";
                    e.Row.Cells[9].HorizontalAlign = HorizontalAlign.Center;
                }
                if (c12.Contains("0,0 MB"))
                {
                    e.Row.Cells[10].Text = "-";
                    e.Row.Cells[10].HorizontalAlign = HorizontalAlign.Center;
                }
                if (c13.Contains("-"))
                {
                    e.Row.Cells[11].Text = "-";
                    e.Row.Cells[11].HorizontalAlign = HorizontalAlign.Center;
                }
                if (c14 == "01/01/0001 00:00")
                {
                    e.Row.Cells[12].Text = "-";
                    e.Row.Cells[12].HorizontalAlign = HorizontalAlign.Center;
                }
                if (c15 == "01/01/0001 00:00")
                {
                    e.Row.Cells[13].Text = "-";
                    e.Row.Cells[13].HorizontalAlign = HorizontalAlign.Center;
                }

                for (int i = 2; i < e.Row.Cells.Count -2; i++)
                {
                    string value = e.Row.Cells[i].Text;
                    if (string.IsNullOrEmpty(value) || value == "01/01/0001 00:00" || value == "0,0 MB" || value == "0,0 min" || value == "0,0 MB/min" || value == "N/A")
                    {
                        e.Row.Cells[i].Text = "-";
                        e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Center;
                    }
                }
            }
        }

        public void gvRelatorios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Copiar")
            {
                try
                {
                    int idCarro = Convert.ToInt32(Server.HtmlDecode(gvRelatorio.Rows[Convert.ToInt32(e.CommandArgument)].Cells[2].Text));
                    DataSet ds = _camadaDados.RealizaConsultaSql(string.Format("select * from carros where id_carro={0}", idCarro));
                    string ip = ds.Tables[0].Rows[0]["IP"].ToString();
                    if (string.IsNullOrEmpty(ip))
                        throw new Exception("O carro selecionado não teve seu ip cadastrado");
                    string usuario = ds.Tables[0].Rows[0]["USUARIO"].ToString();
                    string senha = ds.Tables[0].Rows[0]["SENHA"].ToString();
                    PingaIp(ip);
                    TestaConexaoFtp(ip, usuario, senha);
                    new CamadaControle().ForcaCopia(idCarro);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "createBox('Aviso do Sistema','Carro enviado para cópia com sucesso.<br><b>Aguarde o início da cópia.</b>');", true);
                    CodigoSistema cod = CamadaConfiguracao.COD_38301;
                    Util.RegistraEventoUsuario(this, Convert.ToInt32(cod.Id), cod.Mensagem + " - " + idCarro);
                    ((Button)gvRelatorio.Rows[Convert.ToInt32(e.CommandArgument)].Cells[1].Controls[0]).Enabled = false;
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
        }

        public void btnProximo_Click(object sender, EventArgs e)
        {
            gvRelatorio.PageIndex = ++gvRelatorio.PageIndex;
            gvRelatorio.DataBind();
            lblPaginas.Text = string.Format("{0} de {1}", gvRelatorio.PageIndex + 1, gvRelatorio.PageCount);
            HabilitaDesabilitaBotoes();
        }

        public void btnAnterior_Click(object sender, EventArgs e)
        {
            gvRelatorio.PageIndex = --gvRelatorio.PageIndex;
            gvRelatorio.DataBind();
            lblPaginas.Text = string.Format("{0} de {1}", gvRelatorio.PageIndex + 1, gvRelatorio.PageCount);
            HabilitaDesabilitaBotoes();
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            gvRelatorio.DataSource = FiltrarRelatorio();
            gvRelatorio.DataBind();

        }

        public void btnAdicionarEmail_Click(object sender, EventArgs e)
        {
            try
            {
                StreamWriter sw = new StreamWriter(File.Open(CamadaConfiguracao.CAMINHO_EMAILS, FileMode.Append));
                string[] emails = File.ReadAllLines(CamadaConfiguracao.CAMINHO_EMAILS);
                for (int i = 0; i < emails.Length; i++)
                {
                    if (emails[i] == txtAdicionarEmail.Text)
                    {
                        throw new Exception("O email já foi cadastrado");
                    }
                }
                sw.WriteLine(txtAdicionarEmail.Text);
                sw.Flush();
                sw.Close();
                sw.Dispose();
                sw = null;
                this._emails = CarregaEmails();
                gvEmails.DataSource = this._emails;
                gvEmails.DataBind();
                //create box
            }
            catch
            {
                //create box                
            }
        }

        public void gvEmails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string email = Server.HtmlDecode(gvEmails.Rows[Convert.ToInt32(e.CommandArgument)].Cells[1].Text);
            if (e.CommandName == "remover")
            {
                try
                {
                    string[] emails = File.ReadAllLines(CamadaConfiguracao.CAMINHO_EMAILS);
                    List<string> emailsNovos = new List<string>();
                    for (int i = 0; i < emails.Length; i++)
                    {
                        if (emails[i] != email)
                            emailsNovos.Add(emails[i]);
                    }
                    File.WriteAllLines(CamadaConfiguracao.CAMINHO_EMAILS, emailsNovos.ToArray());
                    this._emails = CarregaEmails();
                    gvEmails.DataSource = this._emails;
                    gvEmails.DataBind();
                }
                catch
                {
                }
            }

            else if (e.CommandName == "Enviar")
            {
                EnviarEmail(email, gvRelatorio);
            }
        }

        public List<RelatorioCopiaItem> FiltrarRelatorio()
        {
            List<RelatorioCopiaItem> filtro = new List<RelatorioCopiaItem>();
            filtro = this._relatorio;
            if (!string.IsNullOrEmpty(txtNumero.Text))
            {
                filtro = filtro.Where(ri => ri.NumeroCarro == Convert.ToInt32(txtNumero.Text)).ToList();
            }
            if (!string.IsNullOrEmpty(txtDataInicial.Text))
            {
                DateTime data = DateTime.Parse(txtDataInicial.Text);
                filtro = filtro.Where(ri => ri.DataInicial >= data).ToList();
            }
            if (!string.IsNullOrEmpty(txtDataFinal.Text))
            {
                DateTime data = DateTime.Parse(txtDataFinal.Text);
                filtro = filtro.Where(ri => ri.DataInicial <= data.AddDays(1)).ToList();
            }
            if (!string.IsNullOrEmpty(txtVelMediaMin.Text))
            {
                double velMedia = Convert.ToDouble(txtVelMediaMin.Text);
                filtro = filtro.Where(ri => ri.VelocidadeMedia >= velMedia).ToList();
            }
            if (!string.IsNullOrEmpty(txtVelMediaMax.Text))
            {
                double velMedia = Convert.ToDouble(txtVelMediaMax.Text);
                filtro = filtro.Where(ri => ri.VelocidadeMedia <= velMedia).ToList();
            }
            if (!string.IsNullOrEmpty(txtDurMin.Text))
            {
                double dur = Convert.ToDouble(txtDurMin.Text);
                filtro = filtro.Where(ri => ri.Intervalo >= dur).ToList();
            }
            if (!string.IsNullOrEmpty(txtDurMax.Text))
            {
                double dur = Convert.ToDouble(txtDurMax.Text);
                filtro = filtro.Where(ri => ri.Intervalo <= dur).ToList();
            }
            if (ddlTipo.SelectedValue != "-999")
            {
                if (ddlTipo.SelectedValue == "0")
                {
                    filtro = filtro.Where(ri => ri.TipoCopia == "Ult Arq").ToList();
                }
                else if (ddlTipo.SelectedValue == "1")
                {
                    filtro = filtro.Where(ri => ri.TipoCopia == "Completa").ToList();
                }
                else if (ddlTipo.SelectedValue == "-1")
                {
                    filtro = filtro.Where(ri => ri.TipoCopia == "Guia").ToList();
                }
            }
            return filtro;
        }

    }


    public struct Email
    {
        public string Endereco { get; set; }
    }
}