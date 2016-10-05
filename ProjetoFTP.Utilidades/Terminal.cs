using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Net.NetworkInformation;
using System.IO;

namespace ProjetoFTP.Utilidades
{
    public class Terminal
    {
        public string Ip { get; set; }
        public string Nome { get; set; }
        public int TamanhoFila { get; set; }
        public bool Comunicando { get; set; }
        public string Diretorio { get; set; }
        public bool Habilitado { get; set; }

        private CamadaControle _camadaControle;
        private CamadaDados _camadaDados;

        public Terminal(string nome, string ip)
        {
            this.Ip = ip;
            this.Nome = nome;
            this._camadaControle = new CamadaControle();
            this._camadaDados = new CamadaDados();
            this.Habilitado = (ip == "localhost") ? false : (new CamadaDados().RealizaConsultaSql(string.Format("SELECT HABILITADA FROM ESTACOES WHERE IP='{0}'", this.Ip)).Tables[0].Rows[0][0].ToString() == "0") ? false : true;
            //this.TamanhoFila = GetTamanhoFila();
        }

        public void EnviaCarroFila(Carro carro)
        {
            EnviarCarroFila(carro.Numero);
        }

        public void EnviarCarroFila(int numero)
        {
            string path = string.Format("{0}{1}.TRM", CamadaConfiguracao.DIRETORIO_TERMINAIS, Ip.Replace('.', '-'));
            
            string value = numero.ToString();

            this._camadaDados.ConcatenaItemTrm(path, value);
            
            TamanhoFila++;

            Log execlog = new Log();
            //execlog.GravalogExec("servidor.log", "Terminal - EnviaCarroFila - Carro : " + value + " - Terminal : " + path);

        }

        public void VerificaComunicação()
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://" + CamadaConfiguracao.IP_CENTRAL + "/queries/GetSignal.ashx");
                    request.Timeout = 1000;
                    request.Method = "POST";
                    StreamWriter sw = new StreamWriter(request.GetRequestStream());
                    sw.Write(this.Ip.Replace('.', '-'));
                    sw.Flush();
                    sw.Close();
                    WebResponse response = request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string s = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    request = null;
                    if (s == "1")
                    {
                        Comunicando = true;
                    }
                    else
                    {
                        Comunicando = false;
                        break;
                    }
                }
                catch(Exception ex)
                {
                    Comunicando = false;
                    Log execlog = new Log();
                    //execlog.GravalogExec("terminal.log", "Terminal - Exception 001 - VerificaComunicacao : " + ex.Message);
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private int GetTamanhoFila()
        {
            return GetCarrosFila().Count;
        }

        public void RemoveCarro(Carro carro)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://" + CamadaConfiguracao.IP_CENTRAL + "/queries/RemoveCarroTerminal.ashx");
                request.Method = "POST";
                StreamWriter writer = new StreamWriter(request.GetRequestStream());
                writer.WriteLine(this.Ip + ";" + carro.Numero);
                writer.Flush();
                writer.Close();
                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string s = sr.ReadToEnd();
                sr.Close();
                response.Close();
                request = null;
                //this._camadaControle.RemoveCarroLista(string.Format("{0}{1}.{2}", CamadaConfiguracao.COMP_DIRETORIO_TERMINAIS, this.Ip.Replace('.', '-'), CamadaConfiguracao.EXTENSAO_TERMINAL), carro);
                this.TamanhoFila--;
                //Console.WriteLine(string.Format("{0} - {1} saiu da fila", DateTime.Now.ToString("HH:mm:ss"), carro.Numero), false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("TERMINAL : RemoveCarro - " + ex.Message + " - " + ex.StackTrace);
            }
        }

        public List<string> GetCarrosFila()
        {
            return Terminal.GetCarrosFila(this.Ip);
        }

        public bool Ping()
        {
            Ping png = new Ping();
            PingReply pr;
            try
            {
                pr = png.Send(Ip);
                if (pr.Status == IPStatus.Success)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public static List<string> GetCarrosFila(string terminal)
        {
            try
            {
                List<string> ret = new List<string>();

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://" + CamadaConfiguracao.IP_CENTRAL + "/queries/GetCarrosTerminal.ashx?t=" + terminal.Replace('.', '-'));

                //Console.WriteLine("http://" + CamadaConfiguracao.IP_CENTRAL + "/queries/GetCarrosTerminal.ashx?t=" + terminal.Replace('.', '-'));

                try
                {
                    WebResponse response = request.GetResponse();

                    if (response.ContentLength > 0)
                    {
                        StreamReader sr = new StreamReader(response.GetResponseStream());
                        string s = sr.ReadToEnd();
                        sr.Close();
                        sr.Dispose();
                        response.Close();
                        request = null;

                        string[] tokens = s.Split(';');
                        if (tokens.Length > 0)
                        {
                            ret = tokens.ToList().GetRange(0, tokens.Length - 1);
                        }
                    }
                    else
                    {
                        Log execlog = new Log();
                        //execlog.GravalogExec("terminal.log", "Terminal - GetCarrosFila - WebResponse É NULL");

                        request = null;
                        response.Close();
                    }
                }
                catch (Exception ex)
                {
                    Log execlog = new Log();
                    //execlog.GravalogExec("servidor.log", "Terminal - GetCarrosFila - WebResponse : " + ex.Message + " - " + ex.StackTrace);
                }

                return ret;
            }
            catch (Exception ex)
            {
                Log execlog = new Log();
                //execlog.GravalogExec("servidor.log", "Terminal - GetCarrosFila : " + ex.Message + " - " + ex.StackTrace);
                throw new Exception("Terminal - GetCarrosFila :" + ex.Message);
            }
        }

        public static Terminal Localhost
        {
            get
            {
                return new Terminal("localhost", "localhost");
            }
        }
    }
}
