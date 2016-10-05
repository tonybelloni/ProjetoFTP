using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ProjetoFTP.Utilidades;

namespace ProjetoFTP.Web.queries
{
    /// <summary>
    /// Summary description for RemoveCarroTerminal
    /// </summary>
    public class RemoveCarroTerminal : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            StreamReader reader = new StreamReader(context.Request.InputStream);
            string[] args = reader.ReadToEnd().Split(';');
            string terminal = args[0];
            string carro = args[1].Remove(args[1].Length - 2, 2).Trim();
            reader.Close();
            try
            {
                string path = @"c:\rioservice\ftp\servidor\terminais\" + terminal.Replace('.','-') + ".TRM";
                string[] linhas = File.ReadAllLines(path);
                List<string> linhasNovas = new List<string>();
                string encryptValue = Criptografia.Encrypt(carro, true);
                for (int i = 0; i < linhas.Length; i++)
                {
                    if (!linhas[i].Contains(encryptValue))
                        linhasNovas.Add(linhas[i]);
                }
                File.WriteAllLines(path, linhasNovas.ToArray());
                context.Response.Write(args[0] + "\r\n");
                context.Response.Write(args[1] + "\r\n");
                context.Response.Write(carro + "\r\n");
                context.Response.Write(encryptValue + "\r\n");
            }
            catch(Exception ex)
            {
                context.Response.Write(ex.Message +";"+ ex.StackTrace);
                Console.Write("TERMINAL : RemoveCarroTerminal : " + ex.Message + " - " + ex.StackTrace);
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