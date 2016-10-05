using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using ProjetoFTP.Utilidades;
using System.IO;
using Microsoft.Win32;

namespace ProjetoFTP.Watchdog
{
    class Program
    {
        private static string processName = "busvisionterminal";
        private static string processPath = Directory.GetCurrentDirectory();
        private static string logPath = "terminal_status";
        static void Main(string[] args)
        {
            string[] tokens = File.ReadAllLines(processPath + @"\wdconfig.config");
            processName = tokens[0];
            logPath = tokens[1];
            Log log = new Log(processPath + @"\" + logPath + ".log");
            log.AddLog("Watchdog inicializado");
            RegistryKey add = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            add.SetValue(processName + "_wd", "\"" + processPath + "\"");
            while (true)
            {
                try
                {
                    Process[] process = Process.GetProcessesByName(processName);
                    if (process.Length > 0)
                    {
                        //processo está rodando
                        Process servidor = process[0];
                        if (!servidor.Responding || servidor.ExitCode != 0)
                        {
                            log.AddLog(processName + " está sendo reinicializado");
                            servidor.Kill();
                            servidor.Start();
                            log.AddLog(processName + " inicializado");
                        }
                    }
                    else
                    {
                        //processo não estava rodando
                        log.AddLog(processName + " está sendo inicializado");
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = processName + ".exe";
                        startInfo.WorkingDirectory = processPath;
                        startInfo.WindowStyle = ProcessWindowStyle.Normal;
                        Process.Start(startInfo);
                        log.AddLog(processName + " inicializado");
                    }
                }
                catch
                {
                }
                Thread.Sleep(10000);
            }

        }
    }
}
