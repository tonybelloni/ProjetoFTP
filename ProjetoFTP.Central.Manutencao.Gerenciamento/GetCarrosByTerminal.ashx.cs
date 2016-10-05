using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
namespace ProjetoFTP.Central.Manutencao.Gerenciamento
{
    /// <summary>
    /// Summary description for GetCarrosByTerminal
    /// </summary>
    public class GetCarrosByTerminal : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string terminal = context.Request.QueryString["t"];
            if (!string.IsNullOrEmpty(terminal))
            {
                try
                {
                    string[] carros = File.ReadAllLines(@"C:\Rio Service\FTP\terminais\" + terminal + ".trm");
                    if (carros.Length > 0)
                    {
                        for (int i = 0; i < carros.Length; i++)
                        {
                            context.Response.Write(carros[i] + "\n");
                        }
                    }
                    else throw new Exception();
                }
                catch
                {
                }
            }
            else
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