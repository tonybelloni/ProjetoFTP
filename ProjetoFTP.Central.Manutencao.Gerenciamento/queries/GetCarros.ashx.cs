using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjetoFTP.Utilidades;
using System.IO;

namespace ProjetoFTP.Web.queries
{
    /// <summary>
    /// Summary description for GetCarros
    /// </summary>
    public class GetCarros : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            string json = CamadaDados.LerArquivo(@"c:\rioservice\ftp\servidor\data.json");
            context.Response.Write(json);
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