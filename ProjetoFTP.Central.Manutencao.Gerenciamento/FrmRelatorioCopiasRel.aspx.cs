using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ProjetoFTP.Utilidades;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Net.NetworkInformation;

namespace ProjetoFTP.Web
{
    public partial class FrmRelatorioCopiasRel : System.Web.UI.Page
    {

        private List<RelatorioCopiaItem> relatorio;
        private List<Email> emails;
        private CamadaDados _camadaDados;
        private string _ultimaCor;
        private int _qtCorSeq;

        protected void Page_Load(object sender, EventArgs e)
        {
            Util.RegistraEntradaUsuario(this);
            Util.VerificaSessaoUsuario(this, "usuario");
            relatorio = new List<RelatorioCopiaItem>();
            this._camadaDados = new CamadaDados();
            if (!IsPostBack)
            {
                txtDataFinal.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtDataInicial.Text = DateTime.Now.Subtract(new TimeSpan(120,0,0,0)).ToString("dd/MM/yyyy");
            }
            CarregaRelatorio();
            emails = CarregaEmails();
            gvEmails.DataSource = emails;
            gvEmails.DataBind();
            gvRelatorio.DataSource = FiltrarRelatorio();
            gvRelatorio.DataBind();
            lblPaginas.Text = string.Format("{0} de {1}",gvRelatorio.PageIndex + 1, gvRelatorio.PageCount);
            HabilitaDesabilitaBotoes();
        }

        public void CarregaRelatorio()
        {
            DataTable dt = this._camadaDados.RealizaConsultaSql("SELECT * FROM COPIAS_HISTORICOS ch WHERE ch.DATA_INICIO_COPIA > sysdate - 365 ORDER BY ch.DATA_INICIO_COPIA DESC").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                RelatorioCopiaItem item = new RelatorioCopiaItem()
                {
                    DataInicial = DateTime.Parse(dt.Rows[i]["DATA_INICIO_COPIA"].ToString()),
                    DataFinal = DateTime.Parse(dt.Rows[i]["DATA_FIM_COPIA"].ToString()),
                    NumeroCarro = Convert.ToInt32(dt.Rows[i]["ID_CARRO"].ToString()),
                    PenDrive = dt.Rows[i]["COD_EQUIPAMENTO"].ToString(),
                    QuantidadeArquivosValidos = (dt.Rows[i]["NUMERO_ARQUIVOS_VALIDOS"]== DBNull.Value) ? 0 : Convert.ToInt32(dt.Rows[i]["NUMERO_ARQUIVOS_VALIDOS"]),
                    QuantidadeArquivosCopiados = Convert.ToInt32(dt.Rows[i]["NUMERO_ARQUIVOS_COPIADOS"].ToString()),
                    QuantidadeArquivosTotal = Convert.ToInt32(dt.Rows[i]["NUMERO_ARQUIVOS_TOTAL"].ToString()),
                    VolumeArquivosCopiados = Convert.ToInt64(dt.Rows[i]["TAMANHO_ARQUIVOS_COPIADOS"].ToString()),
                    VolumeArquivosTotal = Convert.ToInt64(dt.Rows[i]["TAMANHO_ARQUIVOS_TOTAL"].ToString()),
                    PeriodoInicial = (dt.Rows[i]["PERIODO_INICIAL"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(dt.Rows[i]["PERIODO_INICIAL"].ToString()),
                    PeriodoFinal = (dt.Rows[i]["PERIODO_FINAL"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(dt.Rows[i]["PERIODO_FINAL"].ToString()),
                    TipoCopia = (Convert.ToInt32(dt.Rows[i]["TIPO_COPIA"].ToString()) == -1) ? "Guia" : ((Convert.ToInt32(dt.Rows[i]["TIPO_COPIA"].ToString()) == 0) ? "Ult Arq" : "Completa"),
                    Codigo = (dt.Rows[i]["CODIGO_RESULTADO"] == DBNull.Value) ? "N/A" : dt.Rows[i]["CODIGO_RESULTADO"].ToString(),
                    Usuario = "Busvision Gateway Server"
                };
                relatorio.Add(item);
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

        public List<RelatorioCopiaItem> FiltrarRelatorio()
        {
            List<RelatorioCopiaItem> filtro = new List<RelatorioCopiaItem>();
            filtro = relatorio;
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

        public void ExportPDF(GridView grid, string filename, string telDDD)
        {
            try
            {
                HttpResponse Response = grid.Page.Response;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition",
                    "attachment;filename=" + filename + ".pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                grid.AllowPaging = false;
                grid.DataBind();
                grid.RenderControl(hw);
                StringReader sr = new StringReader(sw.ToString());
                //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                //if (!Directory.Exists("C:\\Contas\\" + telDDD.Split(';')[0] + telDDD.Split(';')[1]))
                //    Directory.CreateDirectory("C:\\Contas\\" + telDDD.Split(';')[0] + telDDD.Split(';')[1]);
                //PdfWriter.GetInstance(pdfDoc, new FileStream("C:\\Contas\\" + telDDD.Split(';')[0] + telDDD.Split(';')[1] + @"\" + filename + ".pdf", FileMode.Create));
                //pdfDoc.Open();
                //htmlparser.Parse(sr);
                //pdfDoc.Close();
                FileInfo arquivo = new FileInfo("C:\\Contas\\" + telDDD.Split(';')[0] + telDDD.Split(';')[1] + @"\" + filename + ".pdf");
                Response.WriteFile(arquivo.FullName);
            }
            catch
            { }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Verifies that the control is rendered */
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

        public static void PingaIp(string ip)
        {
            Ping png = new Ping();
            PingReply pr;
            pr = png.Send(ip);
            if (pr.Status != IPStatus.Success)
                throw new Exception("O carro não está comunicando.<br><ul class='lista_procedimentos'><li><b>Procedimentos:</b></li></ul>");
        }

        public static void TestaConexaoFtp(string ip, string usuario, string senha)
        {
            try
            {
                FTP ftp = new FTP(ip, usuario, senha);
                string s = ftp.GetBannerMessage();
            }
            catch
            {
                throw new Exception("Não foi possível fazer acesso FTP no veículo.");
            }
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

        protected void gvRelatorios_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType != DataControlRowType.Header && e.Row.RowType != DataControlRowType.Footer && e.Row.RowType != DataControlRowType.Pager)
            {
                if (Convert.ToInt32(e.Row.Cells[10].Text) == 0)
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
                    this._ultimaCor = "vermelho";
                    
                }
                else
                {
                    if (Convert.ToInt32(e.Row.Cells[10].Text) < Convert.ToInt32(e.Row.Cells[9].Text) * 0.75)
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
                        this._ultimaCor = "amarelo";
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
                        this._ultimaCor = "verde";
                    }
                }
                string c5 = Server.HtmlDecode(e.Row.Cells[5].Text).Trim();
                string c6 = Server.HtmlDecode(e.Row.Cells[6].Text).Trim();
                string c7 = Server.HtmlDecode(e.Row.Cells[7].Text).Trim();
                string c8 = Server.HtmlDecode(e.Row.Cells[8].Text).Trim();
                string c9 = Server.HtmlDecode(e.Row.Cells[9].Text).Trim();
                string c10 = Server.HtmlDecode(e.Row.Cells[10].Text).Trim();
                string c11 = Server.HtmlDecode(e.Row.Cells[11].Text).Trim();
                string c12 = Server.HtmlDecode(e.Row.Cells[12].Text).Trim();
                string c13 = Server.HtmlDecode(e.Row.Cells[13].Text).Trim();
                string c14 = Server.HtmlDecode(e.Row.Cells[14].Text).Trim();
                string c15 = Server.HtmlDecode(e.Row.Cells[14].Text).Trim();
                if (string.IsNullOrEmpty(c6))
                {
                    e.Row.Cells[6].Text = "N/A";
                }
                if (c7.Contains("0,0 min"))
                {
                    e.Row.Cells[7].Text = "-";
                    e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Center;
                }
                if (c11.Contains("0,0 MB"))
                {
                    e.Row.Cells[11].Text = "-";
                    e.Row.Cells[11].HorizontalAlign = HorizontalAlign.Center;
                }
                if (c11.Contains("0,0 MB"))
                {
                    e.Row.Cells[11].Text = "-";
                    e.Row.Cells[11].HorizontalAlign = HorizontalAlign.Center;
                }
                if (c12.Contains("0,0 MB"))
                {
                    e.Row.Cells[12].Text = "-";
                    e.Row.Cells[12].HorizontalAlign = HorizontalAlign.Center;
                }
                if (c13.Contains("0,0 MB/min"))
                {
                    e.Row.Cells[13].Text = "-";
                    e.Row.Cells[13].HorizontalAlign = HorizontalAlign.Center;
                }
                if (c14 == "01/01/0001 00:00")
                {
                    e.Row.Cells[14].Text = "-";
                    e.Row.Cells[14].HorizontalAlign = HorizontalAlign.Center;
                }
                if (c15 == "01/01/0001 00:00")
                {
                    e.Row.Cells[15].Text = "-";
                    e.Row.Cells[15].HorizontalAlign = HorizontalAlign.Center;
                }
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            gvRelatorio.DataSource = FiltrarRelatorio();
            gvRelatorio.DataBind();

        }

        public void gvRelatorios_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRelatorio.PageIndex = e.NewPageIndex;
            gvRelatorio.DataBind();
            System.Threading.Thread.Sleep(1000);
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
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "jAlert( 'Carro enviado para cópia com sucesso.<br><b>Aguarde o início da cópia.</b>','Aviso do Sistema');", true);
                    CodigoSistema cod = CamadaConfiguracao.COD_38301;
                    Util.RegistraEventoUsuario(this, Convert.ToInt32(cod.Id), cod.Mensagem + " - " + idCarro);
                    ((Button)gvRelatorio.Rows[Convert.ToInt32(e.CommandArgument)].Cells[1].Controls[0]).Enabled = false;
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp", "jAlert( '" + ex.Message + "','Aviso do Sistema');", true);
                }
            }
        }

        public void gvEmails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Enviar")
            {
                string email = Server.HtmlDecode(gvEmails.Rows[Convert.ToInt32(e.CommandArgument)].Cells[0].Text);
                EnviarEmail(email, gvRelatorio);
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
    }

}