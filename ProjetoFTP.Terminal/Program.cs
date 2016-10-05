using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ProjetoFTP.Utilidades;
using System.Net;
//using System.Web;
using System.IO;
using Newtonsoft.Json;

namespace ProjetoFTP.Terminal
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                outputmessage("Abrindo aplicativo");
                
                CamadaControle camadaControle = new CamadaControle();
                
                ProjetoFTP.Utilidades.Terminal terminal = null;

                ParametrosConfiguracao config = CamadaConfiguracao.GetConfiguracoes();
                
                string ip = config.TerminalHost;

                outputmessage("IP Terminal : " + ip);
               
                try
                {
                    terminal = camadaControle.GetTerminalByIp(ip);
                    Console.WriteLine(terminal.Nome);
                }
                catch (Exception ex)
                {
                    outputmessage("ERRO : Main - GetTerminalByIP : " + ex.Message);
                }
                AvisaCentral(terminal);

                outputmessage("Terminal conectado com o servidor");
                EscutaLista(terminal);
            }
            catch (Exception ex)
            {
                outputmessage("ERRO : Main : " + ex.Message);
            }
        }

        public static void outputmessage(String mensagem)
        {
            String filename = "terminal.log";
            Log logexec = new Log();

            Console.WriteLine(DateTime.Now.ToString() + " : " +  mensagem);
            logexec.GravalogExec(filename, mensagem);
        }

        public static void EscutaLista(ProjetoFTP.Utilidades.Terminal terminal)
        {
            outputmessage("Escutando terminal....");

            CamadaControle camadaControle = new CamadaControle();
            int qtCopias = 0;
            List<Carro> carros = new List<Carro>();
            
            outputmessage("Recuperando lista de carros....");
            camadaControle.GetCarrosTerminal(terminal, carros);
            outputmessage("Processando lista de carros....");
            
            while (true)
            {
                try
                {
                    outputmessage("loop principal - Verificando carros");

                    terminal.TamanhoFila = carros.Count;

                    outputmessage("Tamanho Fila = " + terminal.TamanhoFila.ToString());

                    for (int i = 0; i < carros.Count; i++)
                    {
                        Carro carro = carros[i];
                        if (!carro.CopiandoNoTerminal && qtCopias < CamadaConfiguracao.QT_COPIAS_SIMULTANEAS)
                        {
                            qtCopias++;
#region threadcopia
                            new Thread((ThreadStart)delegate
                            {
                                TipoCopia tipo = TipoCopia.Completa;
                                try
                                {
                                    outputmessage("Thread copia : Copiando carro : " + carro.Ip + " - " + carro.Numero.ToString());

                                    carro.CopiandoNoTerminal = true;
                                    tipo = carro.CopiaArquivos();
                                }
                                catch(Exception ex)
                                {
                                    outputmessage("Thread copia - Erro na cópia : " + ex.Message);
                                }
                                finally
                                {
                                    try
                                    {
                                        outputmessage("Thread copia - Gravando historico banco de dados");

                                        camadaControle.RegistraLogCopia(new object[] { carro.DataInicial, 
                                                                                   carro.DataFinal, 
                                                                                   carro.QuantidadeArquivosTotal, 
                                                                                   carro.QuantidadeArquivosCopiados, 
                                                                                   carro.VolumeDadosTotal / 1048576, 
                                                                                   carro.VolumeDadosCopiados / 1048576, 
                                                                                   carro.Numero, 
                                                                                   (int)tipo, 
                                                                                   1,
                                                                                   string.IsNullOrEmpty(carro.PenDrive) ? "NULL" : "'" + carro.PenDrive + "'",
                                                                                   carro.QuantidadeArquivosValidos,
                                                                                   carro.PeriodoInicial,
                                                                                   carro.PeriodoFinal,
                                                                                   carro.ServerResponse.Codigo,
                                                                                   "NULL",
                                                                                   terminal.Ip 
                                                                                }
                                                                             );
                                        outputmessage("Thread copia - Historico gravado com sucesso !!");
                                    }
                                    catch(Exception ex)
                                    {
                                        outputmessage("Thread copia - Erro ao gravar historico banco de dados - " + ex.Message);
                                    }

                                    outputmessage("Thread copia - Removendo carro da lista");
                                    terminal.RemoveCarro(carro);

                                    outputmessage("Thread copia - Atualizando informacoes do carro");
                                    camadaControle.AtualizaInformações(carro);

                                    outputmessage("Thread copia - Avisa Central termino da copia");
                                    AvisaCentral(carro);

                                    outputmessage("Thread copia - Finaliza Copia");
                                    carro.CopiandoNoTerminal = false;
                                    carros.Remove(carro);
                                    qtCopias--;
                                }
                            }).Start();
#endregion
                        }
                    }
                    Thread.Sleep(60000);
                }
                catch (Exception ex)
                {
                    outputmessage("Terminal - EscutaFila - Erro 001 : " + ex.Message);
                }
                finally
                {
                    camadaControle.GetCarrosTerminal(terminal, carros);
                }
            }
        }

        public static void AvisaCentral(ProjetoFTP.Utilidades.Terminal t)
        {
            new Thread((ThreadStart)delegate
            {
                 while (true)
                 {
                     try
                     {
                          // DEBUG : outputmessage("AvisaCentral - Terminal - Inicio");
                          HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://" + CamadaConfiguracao.IP_CENTRAL + "/queries/SendSignal.ashx");
                          request.Timeout = 1000;
                          request.Method = "POST";
                          StreamWriter sw = new StreamWriter(request.GetRequestStream());
                          sw.WriteLine(t.Ip);
                          sw.WriteLine(t.Nome);
                          sw.WriteLine(t.TamanhoFila);
                          sw.Flush();
                          sw.Close();
                          WebResponse response = request.GetResponse();
                          StreamReader stream = new StreamReader(response.GetResponseStream());
                          string s = stream.ReadToEnd();
                          stream.Close();
                          response.Close();
                          request = null;
                     }
                     catch (Exception ex)
                     {
                          outputmessage("AvisaCentral - Terminal : Erro ao enviar sinal ao servidor. Terminal fora de rede ou servidor desligado." + ex.Message);
                     }
                     Thread.Sleep(1000);
                 }
            }).Start();
        }

        public static void AvisaCentral(Carro c)
        {
            while (true)
            {
                try
                {
                    outputmessage("AvisaCentral - Carro - Inicio");

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://" + CamadaConfiguracao.IP_CENTRAL + "/queries/SendCarro.ashx");
                    request.Timeout = 10000;
                    request.Method = "POST";
                    StreamWriter sw = new StreamWriter(request.GetRequestStream());
                    sw.WriteLine(c.Numero);
                    sw.Flush();
                    sw.Close();
                    WebResponse response = request.GetResponse();
                    StreamReader stream = new StreamReader(response.GetResponseStream());
                    string s = stream.ReadToEnd();
                    stream.Close();
                    response.Close();
                    request = null;

                    outputmessage("AvisaCentral - Carro - Fim");
                    break;
                }
                catch (Exception ex)
                {
                    outputmessage("AvisaCentral - Carro : Erro ao enviar sinal ao servidor. Terminal fora de rede ou servidor desligado." + ex.Message + " - " + ex.StackTrace);
                }
                Thread.Sleep(20000);
            }
        }
    }
}
