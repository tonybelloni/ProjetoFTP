using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using ProjetoFTP.Utilidades;
using System.Data;

namespace ProjetoFTP.Web.queries
{
    /// <summary>
    /// Summary description for GetTransfInfo
    /// </summary>
    public class GetTransfInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            CamadaDados dados = new CamadaDados();
            int id = Convert.ToInt32(context.Request.QueryString["id"]);
            string query = string.Format("select * from copias_historicos ch left outer join usuarios u on ch.id_usuario = u.id_usuario where ch.ID_COPIA = {0}", id);
            DataSet ds = dados.RealizaConsultaSql(query);
            if (ds.Tables[0].Rows.Count > 0)
            {
                RelatorioCopiaItem rci = new RelatorioCopiaItem()
                {
                    NumeroCarro = Convert.ToInt32(ds.Tables[0].Rows[0]["ID_CARRO"]),
                    DataInicial = DateTime.Parse(ds.Tables[0].Rows[0]["DATA_INICIO_COPIA"].ToString()),
                    DataFinal = DateTime.Parse(ds.Tables[0].Rows[0]["DATA_FIM_COPIA"].ToString()),
                    PenDrive = (ds.Tables[0].Rows[0]["COD_EQUIPAMENTO"] == DBNull.Value) ? "N/A" : ds.Tables[0].Rows[0]["COD_EQUIPAMENTO"].ToString(),
                    PeriodoInicial = (ds.Tables[0].Rows[0]["PERIODO_INICIAL"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(ds.Tables[0].Rows[0]["PERIODO_INICIAL"].ToString()),
                    PeriodoFinal = (ds.Tables[0].Rows[0]["PERIODO_FINAL"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(ds.Tables[0].Rows[0]["PERIODO_FINAL"].ToString()),
                    QuantidadeArquivosCopiados = Convert.ToInt32(ds.Tables[0].Rows[0]["NUMERO_ARQUIVOS_COPIADOS"]),
                    QuantidadeArquivosTotal = Convert.ToInt32(ds.Tables[0].Rows[0]["NUMERO_ARQUIVOS_TOTAL"]),
                    QuantidadeArquivosValidos = Convert.ToInt32(ds.Tables[0].Rows[0]["NUMERO_ARQUIVOS_VALIDOS"]),
                    Codigo = (ds.Tables[0].Rows[0]["CODIGO_RESULTADO"] == DBNull.Value) ? "N/A" : ds.Tables[0].Rows[0]["CODIGO_RESULTADO"].ToString(),
                    TipoCopia = (Convert.ToInt32(ds.Tables[0].Rows[0]["TIPO_COPIA"].ToString()) == -1) ? "Guia" : ((Convert.ToInt32(ds.Tables[0].Rows[0]["TIPO_COPIA"].ToString()) == 0) ? "Ultimo Arquivo" : "Completa"),
                    Usuario = (ds.Tables[0].Rows[0]["USERNAME"] == DBNull.Value ? CamadaConfiguracao.DEFAULT_MANDANTE_COPIA : ds.Tables[0].Rows[0]["USERNAME"].ToString()),
                    VolumeArquivosCopiados = Convert.ToInt64(ds.Tables[0].Rows[0]["TAMANHO_ARQUIVOS_COPIADOS"]),
                    VolumeArquivosTotal = Convert.ToInt64(ds.Tables[0].Rows[0]["TAMANHO_ARQUIVOS_TOTAL"]),
                        
                };
                context.Response.Write(JsonConvert.SerializeObject(rci));
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}