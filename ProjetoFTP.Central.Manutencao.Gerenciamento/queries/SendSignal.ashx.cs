using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using ProjetoFTP.Utilidades;

namespace ProjetoFTP.Web.queries
{
    /// <summary>
    /// Summary description for SendSignal
    /// </summary>
    public class SendSignal : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try
            {
                StreamReader sr = new StreamReader(context.Request.InputStream);
                string response = sr.ReadToEnd();
                sr.Close();
                string[] linhas = response.Split('\n');
                for (int i = 0; i < linhas.Length; i++)
                {
                    linhas[i] = linhas[i].TrimEnd('\r');
                }
                var o = new { ip = linhas[0], nome = linhas[1], fila = Convert.ToInt32(linhas[2]), ultimo_sinal = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };
                string json = JsonConvert.SerializeObject(o);
                CamadaDados.EscreveArquivo(@"c:\rioservice\ftp\servidor\trmjson\" + o.ip.Replace('.', '-') + ".json",json);
            }
            catch(Exception ex)
            {
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