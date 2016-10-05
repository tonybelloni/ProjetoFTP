using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using ProjetoFTP.Utilidades;
using System.IO;
using Newtonsoft.Json;

namespace ProjetoFTP.Web.queries
{
    /// <summary>
    /// Summary description for GetTerminais
    /// </summary>
    public class GetTerminais : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string[] arquivos = Directory.GetFiles(@"c:\rioservice\ftp\servidor\trmjson\");
                List<dynamic> trms = new List<dynamic>();
                for (int i = 0; i < arquivos.Length; i++)
                {
                    string arq = arquivos[i];
                    string f = CamadaDados.LerArquivo(arq);
                    var o = JsonConvert.DeserializeObject(f);
                    trms.Add(o);
                }
                string json = JsonConvert.SerializeObject(
                    new
                    {
                        terminais = trms,
                        data_atual = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                    });
                context.Response.ContentType = "application/json";
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                context.Response.Write(json);
            }
            catch(Exception ex)
            {
                context.Response.ContentType = "text/plain";
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                context.Response.Write(ex.Message);
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