using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjetoFTP.Utilidades;

namespace ProjetoFTP.Central.Manutencao.Gerenciamento.queries
{
    /// <summary>
    /// Summary description for GetPainelHistorico
    /// </summary>
    public class GetPainelHistorico : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            //context.Response.WriteFile(CamadaConfiguracao.DIRETORIO_HISTORICO_PAINEL + "capture20120615113844.json");
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