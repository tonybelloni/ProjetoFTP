using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using ProjetoFTP.Utilidades;
using System.Diagnostics;

namespace ProjetoFTP.Central.Manutencao.Gerenciamento
{
    public partial class FrmLogs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string[] linhas = File.ReadAllLines(@"C:\Rio Service\FTP\logs\teste.txt");
            sw.Stop();
            sw.Start();
            string[] queryResult = linhas.Where(t => t.Contains("abc")).ToArray();
            List<ProjetoFTP.Utilidades.EventLog> logs = new List<ProjetoFTP.Utilidades.EventLog>();
            for (int i = 0; i < queryResult.Length; i++)
            {
                try
                {
                    logs.Add(ProjetoFTP.Utilidades.EventLog.Parse(queryResult[i]));
                }
                catch
                {
                }
            }
            sw.Stop();
            gvLogs.DataSource = logs;
            gvLogs.DataBind();
            GC.Collect();
        }
    }
}