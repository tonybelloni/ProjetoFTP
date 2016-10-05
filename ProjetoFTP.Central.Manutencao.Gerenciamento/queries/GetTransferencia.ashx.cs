using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using ProjetoFTP.Utilidades;

namespace ProjetoFTP.Web.queries
{
    /// <summary>
    /// Summary description for GetTransferencia
    /// </summary>
    public class GetTransferencia : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string cmd = context.Request["a"];
            string file = @"c:\rioservice\ftp\servidor\transferencias\" + context.Request["n"] + ".json";
            if (cmd == "att")
            {
                string json = new StreamReader(context.Request.InputStream).ReadToEnd();
                CamadaDados.EscreveArquivo(file, json);
            }
            else if(cmd == "del")
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            context.Response.Write("");
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