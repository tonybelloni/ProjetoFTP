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
    public partial class FrmRelatorioVerificacoes : System.Web.UI.Page
    {
        private List<RelatorioVerificacaoItem> _relatorio;
        private CamadaDados _dados;
        protected void Page_Load(object sender, EventArgs e)
        {
            Util.VerificaSessaoUsuario(this, "usuario");
            Util.VerificaPermissao(this, new string[] { CamadaConfiguracao.PERMISSAO_ADMINISTRADOR, CamadaConfiguracao.PERMISSAO_VISUALIZADOR });
            Util.AdicionaInfoUsuarioNaTela(form1);
            Util.RegistraEntradaUsuario(this);
            this._dados = new CamadaDados();
            this._relatorio = new List<RelatorioVerificacaoItem>();
            CarregaRelatorio();
            gvRelatorio.DataSource = FiltraRelatorio();
            gvRelatorio.DataBind();
        }

        public void btnFiltrar_Click(object sender, EventArgs e)
        {
            gvRelatorio.DataSource = FiltraRelatorio();
            gvRelatorio.DataBind();
        }

        public List<RelatorioVerificacaoItem> FiltraRelatorio()
        {
            try
            {
                List<RelatorioVerificacaoItem> filtro = new List<RelatorioVerificacaoItem>();
                filtro = this._relatorio;
                if (!string.IsNullOrEmpty(txtNumero.Text))
                {
                    filtro = this._relatorio.Where(f => f.NumeroCarro == Convert.ToInt32(txtNumero.Text)).ToList();
                }
                if (!string.IsNullOrEmpty(txtDtInicial.Text))
                {
                    DateTime data = Convert.ToDateTime(txtDtInicial.Text);
                    filtro = this._relatorio.Where(f => f.DataVerificacao >= data).ToList();
                }
                if (!string.IsNullOrEmpty(txtDtFinal.Text))
                {
                    DateTime data = Convert.ToDateTime(txtDtFinal.Text);
                    filtro = this._relatorio.Where(f => f.DataVerificacao <= data).ToList();
                }
                if (!string.IsNullOrEmpty(txtC1Min.Text))
                {
                    filtro = this._relatorio.Where(f => f.Cam1 >= Convert.ToDouble(txtC1Min.Text)).ToList();
                }
                if (!string.IsNullOrEmpty(txtC1Max.Text))
                {
                    filtro = this._relatorio.Where(f => f.Cam1 <= Convert.ToDouble(txtC1Max.Text)).ToList();
                }
                if (!string.IsNullOrEmpty(txtC2Min.Text))
                {
                    filtro = this._relatorio.Where(f => f.Cam2 >= Convert.ToDouble(txtC2Min.Text)).ToList();
                }
                if (!string.IsNullOrEmpty(txtC2Max.Text))
                {
                    filtro = this._relatorio.Where(f => f.Cam2 <= Convert.ToDouble(txtC2Max.Text)).ToList();
                }
                if (!string.IsNullOrEmpty(txtC3Min.Text))
                {
                    filtro = this._relatorio.Where(f => f.Cam3 >= Convert.ToDouble(txtC3Min.Text)).ToList();
                }
                if (!string.IsNullOrEmpty(txtC3Max.Text))
                {
                    filtro = this._relatorio.Where(f => f.Cam3 <= Convert.ToDouble(txtC3Max.Text)).ToList();
                }
                if (!string.IsNullOrEmpty(txtC4Min.Text))
                {
                    filtro = this._relatorio.Where(f => f.Cam4 >= Convert.ToDouble(txtC4Min.Text)).ToList();
                }
                if (!string.IsNullOrEmpty(txtC4Max.Text))
                {
                    filtro = this._relatorio.Where(f => f.Cam4 <= Convert.ToDouble(txtC4Max.Text)).ToList();
                }
                if (!string.IsNullOrEmpty(txtCTodasMin.Text))
                {
                    filtro = this._relatorio.Where(f => f.Cam1 >= Convert.ToDouble(txtCTodasMin.Text) &&
                                                        f.Cam2 >= Convert.ToDouble(txtCTodasMin.Text) &&
                                                        f.Cam3 >= Convert.ToDouble(txtCTodasMin.Text) &&
                                                        f.Cam4 >= Convert.ToDouble(txtCTodasMin.Text)).ToList();
                }
                if (!string.IsNullOrEmpty(txtCTodasMax.Text))
                {
                    filtro = this._relatorio.Where(f => f.Cam1 <= Convert.ToDouble(txtCTodasMax.Text) &&
                                                        f.Cam2 <= Convert.ToDouble(txtCTodasMax.Text) &&
                                                        f.Cam3 <= Convert.ToDouble(txtCTodasMax.Text) &&
                                                        f.Cam4 <= Convert.ToDouble(txtCTodasMax.Text)).ToList();
                }
                gvRelatorio.DataSource = filtro;
                gvRelatorio.DataBind();
                return filtro; 
            }
            catch
            {
                return null; 
            }
        }

        public void CarregaRelatorio()
        {
            DataTable dt = this._dados.RealizaConsultaSql("SELECT * FROM VERIFICACOES_HISTORICOS ORDER BY DATA_VERIFICACAO DESC").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                RelatorioVerificacaoItem item = new RelatorioVerificacaoItem()
                {
                    NumeroCarro = Convert.ToInt32(dt.Rows[i]["ID_CARRO"]),
                    CodigoEquipamento = "P000000",
                    DataVerificacao = Convert.ToDateTime(dt.Rows[i]["DATA_VERIFICACAO"].ToString()),
                    QtCam1 = Convert.ToInt32(dt.Rows[i]["CAM1_RESULTADO"]),
                    QtCam2 = Convert.ToInt32(dt.Rows[i]["CAM2_RESULTADO"]),
                    QtCam3 = Convert.ToInt32(dt.Rows[i]["CAM3_RESULTADO"]),
                    QtCam4 = Convert.ToInt32(dt.Rows[i]["CAM4_RESULTADO"]),
                    Cam1 = (Convert.ToDouble(dt.Rows[i]["CAM1_RESULTADO"]) / 1440) * 100,
                    Cam2 = (Convert.ToDouble(dt.Rows[i]["CAM2_RESULTADO"]) / 1440) * 100,
                    Cam3 = (Convert.ToDouble(dt.Rows[i]["CAM3_RESULTADO"]) / 1440) * 100,
                    Cam4 = (Convert.ToDouble(dt.Rows[i]["CAM4_RESULTADO"]) / 1440) * 100
                };
                this._relatorio.Add(item);   
            }
        }

        public void gvRelatorios_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRelatorio.PageIndex = e.NewPageIndex;
            gvRelatorio.DataBind();
        }

        public void gvRelatorios_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Server.HtmlDecode(e.Row.Cells[4].Text)[0] == '-')
                {
                    e.Row.Cells[3].Text = "-";
                    e.Row.Cells[4].Text = "-";
                    e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Center;
                }

                if (Server.HtmlDecode(e.Row.Cells[6].Text)[0] == '-')
                {
                    e.Row.Cells[5].Text = "-";
                    e.Row.Cells[6].Text = "-";
                    e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Center;
                }
                if (Server.HtmlDecode(e.Row.Cells[8].Text)[0] == '-')
                {
                    e.Row.Cells[7].Text = "-";
                    e.Row.Cells[8].Text = "-";
                    e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[8].HorizontalAlign = HorizontalAlign.Center;
                }

                if (Server.HtmlDecode(e.Row.Cells[10].Text)[0] == '-')
                {
                    e.Row.Cells[9].Text = "-";
                    e.Row.Cells[10].Text = "-";
                    e.Row.Cells[9].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[10].HorizontalAlign = HorizontalAlign.Center;
                }
            }
        }

    }
}