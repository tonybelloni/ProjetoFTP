using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ProjetoFTP.Utilidades;
using System.Data;
using System.IO;
using iTextSharp.text;
using System.Xml;
using System.Reflection;
using System.Diagnostics;

namespace ProjetoFTP.Web
{
    public class Util
    {
        public Util(Page page)
        {
            if (string.IsNullOrEmpty(page.Header.Title))
                page.Header.Title = CamadaConfiguracao.PAGINA_TITULO_PADRAO;
        }

        public static void AdicionaInfoUsuarioNaTela(Control container)
        {
            HtmlGenericControl control = new HtmlGenericControl("div");
            control.Attributes.Add("id", "container_usuario");
            control.Controls.Add(new System.Web.UI.WebControls.Image() { ID = "imgUsuario", ImageUrl="/img/usuario2.png", Width = 28, Height = 28 });
            control.Controls.Add(new Label() { ID = "lblUsuario", Text = string.Format("{0}",((Usuario)container.Page.Session["usuario"]).Nome) });
            control.Controls.Add(new ImageButton() { ID = "btnLogout", ImageUrl = "/img/fechar3.png", PostBackUrl = "FrmLogout.aspx", ToolTip="Sair" });
            container.Controls.Add(control);
        }

        public static void VerificaSessaoUsuario(Page page, string sessionName)
        {
            if (page.Session[sessionName] == null || string.IsNullOrEmpty(page.Session[sessionName].ToString()))
            {
                page.Response.Redirect("FrmLogin.aspx");
            }
            page.Header.Title = CamadaConfiguracao.PAGINA_TITULO_PADRAO;
        }

        public static void VerificaPermissao(Page page, string[] permissaoNecessaria)
        {
            try
            {
                if (((Usuario)page.Session["usuario"]).Permissao != CamadaConfiguracao.USUARIO_MASTER)
                {
                    string permissao = ((Usuario)page.Session["usuario"]).Permissao;
                    if (string.IsNullOrEmpty(permissao))
                        throw new Exception();
                    if (!permissaoNecessaria.Contains(permissao))
                        throw new Exception();
                }
            }
            catch
            {
                if (page.Session["usuario"] == null) page.Response.Redirect("FrmLogin.aspx");
                page.Response.Redirect("Default.aspx");
            }
        }

        public static void RegistraEntradaUsuario(Page page)
        {
            if (!page.IsPostBack)
            {
                if (page.Session["usuario"] != CamadaConfiguracao.USUARIO_MASTER)
                {
                    string diretorio = string.Format(@"{0}{1}\", CamadaConfiguracao.DIRETORIO_LOGS_USUARIOS, page.Session["usuario"]);
                    if (!Directory.Exists(diretorio))
                        Directory.CreateDirectory(diretorio);
                    string evento = string.Format("{0} @ {1} -> {2}", new object[] { DateTime.Now.ToString("HH:mm:ss"), page.Session["usuario"], page.AppRelativeVirtualPath });
                    try
                    {
                        StreamWriter sw = File.AppendText(diretorio + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
                        sw.WriteLine(evento);
                        sw.Flush();
                        sw.Dispose();
                        sw.Close();
                    }
                    catch
                    {
                        RegistraEntradaUsuario(page);
                    }
                }
            }
        }

        public static void RegistraEventoUsuario(Page page, int codigo, string acao)
        {
            string diretorio = string.Format(@"{0}{1}\", CamadaConfiguracao.DIRETORIO_LOGS_USUARIOS, page.Session["usuario"]);
            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);
            string evento = string.Format("{0} - {1} @ {2} -> {3} -> {4}", new object[] { DateTime.Now.ToString("HH:mm:ss"), codigo, page.Session["usuario"], page.AppRelativeVirtualPath, acao});
            try
            {
                StreamWriter sw = File.AppendText(diretorio + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
                sw.WriteLine(evento);
                sw.Flush();
                sw.Dispose();
                sw.Close();
            }
            catch
            {
                RegistraEntradaUsuario(page);
            }
        }

        public static void GridViewToPdf(GridView gv, string fileName)
        {
            gv.Page.Response.Clear();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            gv.Page.Response.ContentType = "application/pdf";
            gv.Page.Response.AddHeader("content-disposition", "attachment; filename=MypdfFile.pdf");
            Document document = new Document();
            iTextSharp.text.pdf.PdfWriter.GetInstance(document, gv.Page.Response.OutputStream);
            document.Open();
            string html = sb.ToString();
            XmlTextReader reader = new XmlTextReader(new StringReader(html));
            //HtmlParser.Parse(document, reader);

            document.Close();
            sw.Close();
            gv.Page.Response.Flush();
            gv.Page.Response.End();

        }
    }

}