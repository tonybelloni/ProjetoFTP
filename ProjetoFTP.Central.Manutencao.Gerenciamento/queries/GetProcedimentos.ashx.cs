using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjetoFTP.Utilidades;
using Newtonsoft.Json;

namespace ProjetoFTP.Web.queries
{
    /// <summary>
    /// Summary description for GetProcedimentos
    /// </summary>
    public class GetProcedimentos : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            string codigo = context.Request.QueryString["cod"];
            if (!string.IsNullOrEmpty(codigo))
            {
                ServerResponse sr = CamadaConfiguracao.GetServerResponse(codigo);
                context.Response.Write(JsonConvert.SerializeObject(sr));
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