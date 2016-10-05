using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ProjetoFTP.Utilidades;

namespace ProjetoFTP.Web.queries
{
    /// <summary>
    /// Summary description for GetCarrosTerminal
    /// </summary>
    public class GetCarrosTerminal : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            StreamReader sr = new StreamReader(@"c:\rioservice\ftp\servidor\terminais\" + context.Request.QueryString["t"] + ".trm");
            string s = sr.ReadToEnd();
            string[] l = s.Split('\n');
            string r = "";
            for (int i = 0; i < l.Length; i++)
            {
                string c = l[i].TrimEnd('\r');
                try
                {
                    string d = Criptografia.Decrypt(c, true);
                    r += d + ((i == (l.Length - 1)) ? "" : ";");
                }
                catch
                {
                }
            }
            sr.Close();
            sr.Dispose();
            sr = null;
            context.Response.Write(r);
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