using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ProjetoFTP.Utilidades;
//using Highchart.Core;
//using Highchart.Core.Data.Chart;

namespace ProjetoFTP.Web
{
    public partial class FrmRelatorios : System.Web.UI.Page
    {

        private List<RelatorioCopiaItem> relatorio;
        private CamadaDados _camadaDados;

        protected void Page_Load(object sender, EventArgs e)
        {
            Util.VerificaSessaoUsuario(this, "usuario");
            Util.VerificaPermissao(this, new string[] { CamadaConfiguracao.PERMISSAO_ADMINISTRADOR, CamadaConfiguracao.PERMISSAO_VISUALIZADOR });
            Util.AdicionaInfoUsuarioNaTela(form1);
            Util.RegistraEntradaUsuario(this);
            relatorio = new List<RelatorioCopiaItem>();
            this._camadaDados = new CamadaDados();
            CarregaRelatorioVerificacao();
        }

        public void CarregaRelatorioVerificacao()
        {
            DateTime intervalo = DateTime.Now - new TimeSpan(12, 0, 0);
            DataTable semCopiar = this._camadaDados.RealizaConsultaSql(string.Format("SELECT ID_CARRO, ULTIMA_VERIFICACAO, ULTIMA_COPIA FROM CARROS c " +
                                                                                     "WHERE c.ULTIMA_COPIA <= to_date('{0}','DD/MM/YYYY HH24:MI:SS') AND " +
                                                                                     "c.IP is not null order by ULTIMA_COPIA desc",
                                                                                     intervalo.ToString("dd/MM/yyyy HH:mm:ss"))).Tables[0];
            DataTable semVerificar = this._camadaDados.RealizaConsultaSql(string.Format("SELECT ID_CARRO, ULTIMA_VERIFICACAO, ULTIMA_COPIA FROM CARROS c " +
                                                                                        "WHERE c.ULTIMA_VERIFICACAO <= to_date('{0}','DD/MM/YYYY HH24:MI:SS') AND " +
                                                                                        "c.IP is not null order by ULTIMA_VERIFICACAO desc",
                                                                                        intervalo.ToString("dd/MM/yyyy HH:mm:ss"))).Tables[0];
            List<CarroAusente> carrosAusentes = new List<CarroAusente>();
            for (int i = 0; i < semCopiar.Rows.Count; i++)
            {
                CarroAusente carro = new CarroAusente();
                carro.Numero = Convert.ToInt32(semCopiar.Rows[i]["ID_CARRO"]);
                carro.UltimaVerificacao = Convert.ToDateTime(semCopiar.Rows[i]["ULTIMA_VERIFICACAO"]);
                carro.UltimaCopia = Convert.ToDateTime(semCopiar.Rows[i]["ULTIMA_COPIA"]);
                carro.CssClass = "item amarelo";
                carrosAusentes.Add(carro);
            }
            for (int i = 0; i < semVerificar.Rows.Count; i++)
            {
                CarroAusente carro = new CarroAusente();
                carro.Numero = Convert.ToInt32(semVerificar.Rows[i]["ID_CARRO"]);
                carro.UltimaVerificacao = Convert.ToDateTime(semVerificar.Rows[i]["ULTIMA_VERIFICACAO"]);
                carro.UltimaCopia = Convert.ToDateTime(semVerificar.Rows[i]["ULTIMA_COPIA"]);
                CarroAusente aux = carrosAusentes.Find(c => c.Numero == carro.Numero);
                if(aux != null)
                {
                    aux.CssClass = "item laranja";
                    carrosAusentes[carrosAusentes.FindIndex(c => c.Numero == aux.Numero)] = aux;
                }
                else
                {
                    carro.CssClass = "item branco";
                    carrosAusentes.Add(carro);
                }
            }
            //gvVerificacao.DataSource = dt;
            //gvVerificacao.DataBind();
            dlGridCarros.DataSource = carrosAusentes;
            dlGridCarros.DataBind();
            
        }

        public void dlGridCarros_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            Panel panel = (Panel)e.Item.FindControl("pnlItem");
            //panel.CssClass = "item azul";
        }

        public void gvVerificacao_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
        //    gvVerificacao.PageIndex = e.NewPageIndex;
        //    gvVerificacao.DataBind();
        }

    }

    public class CarroAusente
    {
        public int Numero { get; set; }
        public DateTime UltimaCopia { get; set; }
        public DateTime UltimaVerificacao { get; set; }
        public string CssClass { get; set; }
    }
}