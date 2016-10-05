using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ProjetoFTP.Utilidades;

namespace ProjetoFTP.Web.queries
{
    /// <summary>
    /// Summary description for SendCarro
    /// </summary>
    public class SendCarro : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            StreamReader sr = new StreamReader(context.Request.InputStream);
            string response = sr.ReadToEnd();
            sr.Close();
            response = response.Replace("\r", "").Replace("\n", "");
            StreamWriter sw = File.AppendText(@"c:\rioservice\ftp\servidor\terminais\updates.TRM");
            sw.WriteLine(Criptografia.Encrypt(response,true));
            sw.Flush();
            sw.Dispose();
            sw.Close();
            sw = null;
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