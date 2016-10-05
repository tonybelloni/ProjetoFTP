using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjetoFTP.Utilidades;
using Newtonsoft.Json;

namespace ProjetoFTP.Central.Manutencao.Gerenciamento.queries
{
    /// <summary>
    /// Summary description for getServerInfo
    /// </summary>
    public class getServerInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            //context.Response.WriteFile(CamadaConfiguracao.JSON_SERVER_STATUS);
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