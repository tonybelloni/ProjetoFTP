using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjetoFTP.Utilidades;
using Newtonsoft.Json;
using System.IO;
using ProjetoFTP.Utilidades;

namespace ProjetoFTP.Web.queries
{
    /// <summary>
    /// Summary description for GetTransferencias
    /// </summary>
    public class GetTransferencias : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            string[] arquivos = Directory.GetFiles(@"c:\rioservice\ftp\servidor\transferencias\");
            List<dynamic> resultados = new List<dynamic>();
            for (int i = 0; i < arquivos.Length; i++)
            {
                try
                {
                    string s = arquivos[i];
                    string json = CamadaDados.LerArquivo(s);
                    var transf = JsonConvert.DeserializeObject(json);
                    resultados.Add(transf);
                }
                catch
                {
                }
            }
            context.Response.Write(JsonConvert.SerializeObject(resultados));
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