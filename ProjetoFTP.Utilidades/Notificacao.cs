using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace ProjetoFTP.Utilidades
{
    public class Notificacao
    {
        public int Carro { get; set; }
        public DateTime Data { get; set; }
        public string Tipo { get; set; }
        public int Id { get; set; }
        public string Mensagem { get; set; }
        public int Status { get; set; }

        private static Log execlog;

        public void Add()
        {
            Add(this);
        }

        public static void Add(Notificacao notificacao)
        {
            
            execlog = new Log();

            try
            {
                string file = @"c:\rioservice\ftp\servidor\notificacoes\" + DateTime.Now.ToString("yyyyMMdd") + ".json";
                string content = "";
                int lastId = GetLastId();

                if (File.Exists(file))
                    content = CamadaDados.LerArquivo(file);
                
                if (!string.IsNullOrEmpty(content))
                {
                    var json = JsonConvert.DeserializeAnonymousType(content, new
                    {
                        notifications = new List<dynamic>() 
                        { 
                            new 
                            { 
                                id = 0,
                                c = 0,
                                t = "",
                                d = "",
                                m = "",
                                s = ""
                            }
                        }
                    });
                    json.notifications.Add(new
                    {
                        id = ++lastId,
                        c = notificacao.Carro,
                        t = notificacao.Tipo,
                        d = notificacao.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                        m = notificacao.Mensagem,
                        s = notificacao.Status
                    });
                    string newJson = JsonConvert.SerializeObject(json);
                    CamadaDados.EscreveArquivo(file, newJson);
                }
                else
                {
                    var obj = new
                                {
                                    notifications = new List<dynamic>() 
                                { 
                                    new 
                                    { 
                                        id = ++lastId,
                                        c = notificacao.Carro,
                                        t = notificacao.Tipo,
                                        d = notificacao.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                        m = notificacao.Mensagem,
                                        s = notificacao.Status
                                    }
                                }
                                };

                    string json = JsonConvert.SerializeObject(obj);
                    CamadaDados.EscreveArquivo(file, json);
                }


            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                //execlog.GravalogExec("notificacao.log", "Add() - " + ex.Message);
                //Add(notificacao);
            }
        }

        private static int GetLastId()
        {

            execlog = new Log();
            string path = @"c:\rioservice\ftp\servidor\notificacoes\" + DateTime.Now.AddDays(1).ToString("yyyyMMdd") + ".json";

            try
            {
                string content = "";
                string lastPath = Notificacao.GetPathAnterior(path);
                if (lastPath != string.Empty)
                {
                    content = CamadaDados.LerArquivo(lastPath);
                }
                else
                {
                    content = CamadaDados.LerArquivo(path);
                }
                if (!string.IsNullOrEmpty(content))
                {
                    var json = JsonConvert.DeserializeAnonymousType(content, new
                    {
                        notifications = new List<dynamic>() 
                        { 
                            new 
                            { 
                                id = 0,
                                c = 0,
                                t = "",
                                d = "",
                                m = "",
                                s = ""
                            }
                        }
                    });
                    return json.notifications.Max(n => n.id);

                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("notificacao.log", "GetLastId() : " + ex.Message + " - " + path);
                return 0;
            }
        }

        private static string GetPathAnterior(string p)
        {
            execlog = new Log();

            try
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
            catch(Exception ex)
            {
                //execlog.GravalogExec("notificacao.log", "GetPathAnterior() : " + ex.Message);
                return string.Empty;
            }
        }
    }
}
