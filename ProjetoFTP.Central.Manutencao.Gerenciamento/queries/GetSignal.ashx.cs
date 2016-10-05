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
    /// Summary description for GetSignal
    /// </summary>
    public class GetSignal : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            StreamReader reader = new StreamReader(context.Request.InputStream);
            string terminal = reader.ReadToEnd();
            string data = "";
            string json = "json";
            terminal = terminal.Replace("\r", "").Replace("\n", "").Replace('.', '-');
            DateTime d = DateTime.MinValue;
            DateTime n = DateTime.MinValue;
            try
            {
                json = CamadaDados.LerArquivo(@"c:\rioservice\ftp\servidor\trmjson\" + terminal + ".json");
                var o = new { ip = "", nome = "", fila = 0, ultimo_sinal = "" };
                var jsonObject = JsonConvert.DeserializeAnonymousType(json, o);
                d = DateTime.Parse(jsonObject.ultimo_sinal);
                n = DateTime.Now - new TimeSpan(0,0,2);
                if (n < d)
                    context.Response.Write("1");
                else 
                    context.Response.Write("0");
            }
            catch(Exception ex)
            {
                context.Response.Write(data);
                context.Response.Write(json);
                context.Response.Write(ex);
                context.Response.Write(d);
                context.Response.Write(n);
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