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
    /// Summary description for GetNotificacoes
    /// </summary>
    public class GetNotificacoes : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var auxJson = new { notifications = new[] { new { id = 0, t = "", m = "", d = "", c = 0, s = 0 } } };
                string a = context.Request.QueryString["a"];
                string path = string.Empty;
                string content = string.Empty;
                string result = string.Empty;
                int i = Convert.ToInt32(context.Request.QueryString["i"]);
                if (a == "newest")
                {
                    string date = DateTime.Now.ToString("yyyyMMdd");
                    path = @"c:\rioservice\ftp\servidor\notificacoes\" + date + ".json";
                    content = CamadaDados.LerArquivo(path);
                    var json = JsonConvert.DeserializeAnonymousType(content, auxJson);
                    var aux = json.notifications.Where(n => n.id > i);
                    if (i == 0) // pega as 20 ultimas independente do id
                    {
                        if (aux.Count() < 20) // se no dia tiver menos de 20 notificações o outro dia é procurado
                        {
                            path = GetPathAnterior(path);
                            if (path != string.Empty)
                            {
                                content = CamadaDados.LerArquivo(path);
                                var latestJson = JsonConvert.DeserializeAnonymousType(content, auxJson);
                                aux = aux.Concat(latestJson.notifications).OrderBy(n => n.id);
                            }
                        }
                        aux = aux.Reverse().Take(20).Reverse();
                    }
                    json = new { notifications = aux.ToArray() };
                    result = JsonConvert.SerializeObject(json);
                }
                else if (a == "oldest")
                {
                    string date = context.Request.QueryString["d"];
                    path = @"c:\rioservice\ftp\servidor\notificacoes\" + date + ".json";
                    content = CamadaDados.LerArquivo(path);
                    var json = JsonConvert.DeserializeAnonymousType(content, auxJson);
                    var aux = json.notifications.Where(n => n.id < i);
                    if (aux.Count() < 20)
                    {
                        path = GetPathAnterior(path);
                        if (path != string.Empty)
                        {
                            content = CamadaDados.LerArquivo(path);
                            var latestJson = JsonConvert.DeserializeAnonymousType(content, auxJson);
                            aux = aux.Concat(latestJson.notifications).OrderBy(n => n.id);
                        }
                    }
                    aux = aux.Reverse().Take(20);
                    json = new { notifications = aux.ToArray() };
                    result = JsonConvert.SerializeObject(json);
                }
                context.Response.ContentType = "application/json";
                context.Response.Write(result);
            }
            catch
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


        public string GetPathAnterior(string p)
        {
            string[] files = Directory.GetFiles(@"c:\rioservice\ftp\servidor\notificacoes\");
            files = files.OrderByDescending(f => Convert.ToInt32(f.Substring(f.LastIndexOf('\\') + 1, 8))).ToArray();
            for (int i = 0; i < files.Length; i++)
            {
                if (p != files[i])
                    return files[i];
            }
            return string.Empty;
        }
    }
}