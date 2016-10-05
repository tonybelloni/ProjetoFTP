using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProjetoFTP.Utilidades;
using System.IO;

namespace ProjetoFTP.Central.Manutencao.Gerenciamento
{
    public partial class FrmPainel : System.Web.UI.Page
    {
        private CamadaControle _camadaControle;
        private CamadaDados _camadaDados;

        List<XmlTransferencia> transferencias;
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void gvTransferencias_DataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblCarro = (Label)e.Row.Cells[0].FindControl("lblCarro");
                Label lblArquivos = (Label)e.Row.Cells[0].FindControl("lblArquivos");
                Label lblVolume = (Label)e.Row.Cells[0].FindControl("lblVolume");
                Label lblDataInicial = (Label)e.Row.Cells[0].FindControl("lblDataInicial");
                Label lblVelocidade = (Label)e.Row.Cells[0].FindControl("lblVelocidade");
                Label lblTempoRestante = (Label)e.Row.Cells[0].FindControl("lblTempoRestante");
                Panel pnlBarra = (Panel)e.Row.Cells[0].FindControl("pnlUpd");
                Panel pnlContainer = (Panel)e.Row.Cells[0].FindControl("pnlUpdContainer");
                Unit uinit = new Unit((transferencias[e.Row.RowIndex].VolumeCopiado / transferencias[e.Row.RowIndex].VolumeTotal) * pnlContainer.Width.Value, UnitType.Pixel);
                pnlBarra.Width = uinit;
                lblCarro.Text = string.Format("{0}", transferencias[e.Row.RowIndex].Numero);
                lblArquivos.Text = string.Format("{0} arquivos copiados de {1}", transferencias[e.Row.RowIndex].ArquivosCopiados, transferencias[e.Row.RowIndex].ArquivosTotal);
                lblVolume.Text = string.Format("{0}MB copiados de {1}MB", transferencias[e.Row.RowIndex].VolumeCopiado, transferencias[e.Row.RowIndex].VolumeTotal);
                lblVelocidade.Text = string.Format("{0}MB/s", transferencias[e.Row.RowIndex].VelocidadeMedia.ToString("F"));

            }
        }
    }
}
