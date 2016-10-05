using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ProjetoFTP.Utilidades;
using System.IO;
using System.Net.NetworkInformation;
using System.Diagnostics;
using ProjetoFTP.Utilidades.exceptions;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;

namespace ProjetoFTP.Central.Manutencao
{
    public class Program
    {
        static GerenciadorTerminal gerenciadorTerminal;
        static List<Carro> carros;
        static Log log;
        static CamadaControle controle;

        public static void outputmessage(String mensagem)
        {
            String filename = "servidor.log";
            Log logexec = new Log();

            Console.WriteLine(DateTime.Now.ToString() + " : " + mensagem);
            // logexec.GravalogExec(filename, mensagem);
        }

        static void Main(string[] args)
        {
            try
            {
                log = new Log(@"c:\rioservice\ftp\servidor\logs\sends.log");
                Console.Title = "Servidor FTP";

                //Teste GIT

                gerenciadorTerminal = new GerenciadorTerminal();
                controle = new CamadaControle();
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                Console.WriteLine("Servidor - Início - Versão : 2.3.1");
                //outputmessage("Servidor - Início - Versão : 2.3.1");

                carros = controle.GetCarros();

                Console.WriteLine("Servidor - RestauraSistema");
                //outputmessage("Servidor - RestauraSistema");
                RestauraSistema();

                Console.WriteLine("Servidor - RotinaAtualizacaoCarros");
                //outputmessage("Servidor - RotinaAtualizacaoCarros");
                RotinaAtualizacaoCarros();

                Console.WriteLine("Servidor - EscutaChamadasCopias");
                //outputmessage("Servidor - EscutaChamadasCopias");
                EscutaChamadasCopias();

                Console.WriteLine("Servidor - VerificaTerminais");
                //outputmessage("Servidor - VerificaTerminais");
                gerenciadorTerminal.VerificaTerminais();

                Console.WriteLine("Servidor - Inicio loop principal");
                //outputmessage("Servidor - Inicio loop principal");
                for (int i = 0; i < carros.Count; i++)
                {
                    try
                    {
                        //outputmessage("Servidor - IniciaRotinaVerificao - Inicio");
                        IniciaRotinaVerificacao(i);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Servidor - Main - Exception 001 : " + ex.Message + " - " + ex.StackTrace);
                        //outputmessage("Servidor - Main - Exception 001 : " + ex.Message + " - " + ex.StackTrace);
                    }
                }
                RotinaSaidaJson(carros);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //outputmessage(ex.Message);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                Environment.Exit(1);
            }
        }

        public static void IniciaRotinaVerificacao(int i)
        {
            try
            {
                Thread trd = new Thread((ThreadStart)delegate
                {
                    bool pingava = false;
                    TipoDvr ultimoTipo = TipoDvr.Desconhecido;
                    bool gravava = false;
                    bool first = true;
                    int sleep = 60000;
                    int maxNotifSleep = 1800000;
                    int maxUpdateSleep = 60000;
                    int sleepNotifFtp = maxNotifSleep;
                    int sleepNotifGrav = maxNotifSleep;
                    int sleepUpdate = 0;

                    while (true)
                    {
                        //outputmessage("Servidor - IniciaRotinaVerificacao - Inicio loop principal");
                        try
                        {
                            //outputmessage("Servidor - IniciaRotinaVerificacao - Inicio ping...");
                            if (carros[i].Ping())
                            {
                                Console.WriteLine(string.Format("Servidor - IniciaRotinaVerificacao - Carro {0} pingado", carros[i].Ip));
                                //outputmessage(string.Format("Servidor - IniciaRotinaVerificacao - Carro {0} pingado", carros[i].Ip));

                                carros[i].UltimoPing = DateTime.Now;

                                //outputmessage("Servidor - Atualizando dados do carro...");
                                #region atualiza dados do carro

                                if (sleepUpdate >= maxUpdateSleep)
                                {
                                    controle.AtualizaUltimoPing(carros[i]);
                                    Console.WriteLine(string.Format("Servidor - IniciaRotinaVerificacao - 001 : O carro {0} foi atualizado", carros[i].Numero));
                                    //outputmessage(string.Format("Servidor - IniciaRotinaVerificacao - 001 : O carro {0} foi atualizado", carros[i].Numero));
                                    sleepUpdate = 0;
                                }
                                #endregion

                                //outputmessage("Servidor - Notificando status...");
                                #region notifica status do sinal

                                if (!pingava && !first)
                                {
                                    Notificacao.Add(new Notificacao
                                    {
                                        Carro = carros[i].Numero,
                                        Data = DateTime.Now,
                                        Mensagem = "O carro foi localizado novamente",
                                        Tipo = "s",
                                        Status = 1
                                    });
                                }
                                #endregion

                                Console.WriteLine("Servidor - Identificando equipamento...");
                                //outputmessage("Servidor - Identificando equipamento...");
                                carros[i].IdentificaEquipamento();

                                Console.WriteLine("Servidor - Notificando status do FTP...");
                                //outputmessage("Servidor - Notificando status do FTP...");
                                #region notifica status do ftp

                                if (carros[i].TipoDvr == TipoDvr.Desconhecido)
                                {
                                    if (sleepNotifFtp >= maxNotifSleep && pingava)
                                    {
                                        Notificacao.Add(new Notificacao
                                        {
                                            Carro = carros[i].Numero,
                                            Data = DateTime.Now,
                                            Mensagem = "O carro foi localizado, mas o FTP está desligado",
                                            Tipo = "t",
                                            Status = 0
                                        });
                                        sleepNotifFtp = 0;
                                    }
                                }
                                else
                                {
                                    sleepNotifFtp = maxNotifSleep;
                                    if (ultimoTipo == TipoDvr.Desconhecido && !first)
                                    {
                                        Notificacao.Add(new Notificacao
                                        {
                                            Carro = carros[i].Numero,
                                            Data = DateTime.Now,
                                            Mensagem = "O FTP voltou a conectar.",
                                            Tipo = "t",
                                            Status = 1
                                        });
                                    }
                                }
                                #endregion

                                Console.WriteLine("Servidor - Verificando Cameras...");
                                //outputmessage("Servidor - Verificando Cameras...");
                                carros[i].VerificaCameras();

                                lock (gerenciadorTerminal)
                                {
                                    Console.WriteLine("Servidor - Enviando carro para terminal...");
                                    //outputmessage("Servidor - Enviando carro para terminal...");
                                    #region envia carro para o terminal
                                    if ((carros[i].UltimaCopia + new TimeSpan(6, 0, 0) <= DateTime.Now)/* ||
                                    (carros[i].UltimoStatus != StatusCopia.CopiaCompleta && carros[i].TipoDvr != TipoDvr.Desconhecido)*/)
                                    {
                                        // caso o carro tenha copiado normalmente haverá uma pausa de 6h para a próxima cópia. Caso contrário a cópia será feita imediatamente.
                                        //if (carros[i].TemArquivos())
                                        {
                                            try
                                            {
                                                Terminal t = gerenciadorTerminal.EnviaCarro(carros[i]);

                                                if (t != null)
                                                {
                                                    Console.WriteLine("Numero = " + carros[i].Numero.ToString());
                                                    Console.WriteLine("Tipo   = " + carros[i].TipoDvr.ToString());
                                                    Console.WriteLine("Nome   = " + t.Nome.ToString());
                                                    Console.WriteLine("Ultima = " + carros[i].UltimaCopia.ToString());
                                                    Console.WriteLine(string.Format("Servidor - IniciaRotinaVerificacao - 002 : {1} {0} enviado para o terminal {2} - Ultima Cópia: {3}", carros[i].Numero, carros[i].TipoDvr, t.Nome, carros[i].UltimaCopia));
                                                    //outputmessage(string.Format("Servidor - IniciaRotinaVerificacao - 002 : {1} {0} enviado para o terminal {2} - Ultima Cópia: {3}", carros[i].Numero, carros[i].TipoDvr, t.Nome, carros[i].UltimaCopia));
                                                    //log.AddLog(string.Format("Carro {0} enviado para o terminal {1}", carros[i].Numero, t.Nome));
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Carro " + carros[i].Numero + " já está sendo copiado em algum terminal !!!");
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine("Servidor - Exception 999 : " + ex.Message + " - " + ex.StackTrace);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Carro " + carros[i].Numero + " Já foi copiado nas últimas 6 horas !!!");
                                    }
                                    #endregion
                                }

                                #region notifica o status da gravacao
                                if (!carros[i].Gravando)
                                {
                                    if (carros[i].Ping() && pingava)
                                    {   //Verifica se o carro não saiu da garagem no meio do processo de verificação. 
                                        //Se ele pingar, significa que ele realmente tem um problema que indica não estar gravando.
                                        if (sleepNotifGrav >= maxNotifSleep)
                                        {
                                            Notificacao.Add(new Notificacao
                                            {
                                                Carro = carros[i].Numero,
                                                Data = DateTime.Now,
                                                Mensagem = "O carro não está gravando.",
                                                Tipo = "c",
                                                Status = 0
                                            });
                                            sleepNotifGrav = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    sleepNotifGrav = maxNotifSleep;
                                    if (!gravava && !first)
                                    {
                                        Notificacao.Add(new Notificacao
                                        {
                                            Carro = carros[i].Numero,
                                            Data = DateTime.Now,
                                            Mensagem = "O carro voltou a gravar.",
                                            Tipo = "c",
                                            Status = 1
                                        });
                                    }
                                }
                                #endregion

                                pingava = true;
                            }
                            else
                            {
                                Console.WriteLine(string.Format("Servidor - IniciaRotinaVerificacao - Carro {0} não encontrado..", carros[i].Ip));
                                //outputmessage(string.Format("Servidor - IniciaRotinaVerificacao - Carro {0} não encontrado..", carros[i].Ip));

                                #region notifica perda de sinal
                                if (pingava)
                                {
                                    Notificacao.Add(new Notificacao
                                    {
                                        Carro = carros[i].Numero,
                                        Data = DateTime.Now,
                                        Mensagem = "O sinal do carro foi perdido",
                                        Tipo = "s",
                                        Status = 0
                                    });
                                }
                                #endregion

                                pingava = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Servidor - Exception 001 - IniciaRotinaVerificacao : " + ex.Message + " - " + ex.StackTrace);
                            //outputmessage("Servidor - Exception 001 - IniciaRotinaVerificacao : " + ex.Message + " - " + ex.StackTrace);
                        }
                        finally
                        {
                            gravava = carros[i].Gravando;
                            ultimoTipo = carros[i].TipoDvr;
                            first = false;
                            if (sleepNotifFtp < maxNotifSleep) sleepNotifFtp += sleep;
                            if (sleepNotifGrav < maxNotifSleep) sleepNotifGrav += sleep;
                            if (sleepUpdate < maxUpdateSleep) sleepUpdate += sleep;

                            Thread.Sleep(sleep);
                        }
                    }
                });
                trd.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Servidor - IniciaRotinaVerificacao - Exception 002 - " + ex.Message + ex.StackTrace);
                //outputmessage("Servidor - IniciaRotinaVerificacao - Exception 002 - " + ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// Cria uma thread que verifica continuamente se uma requisição de cópia foi enviada pelo usuário. Caso exista alguma requisição o método a identifica, trata e envia ao terminal correto para ser copiada.
        /// </summary>
        public static void EscutaChamadasCopias()
        {
            new Thread((ThreadStart)delegate
            {
                Terminal localhost = Terminal.Localhost;

                while (true)
                {
                    try
                    {
                        List<string> carros = new List<string>();
                        carros = localhost.GetCarrosFila();

                        if (carros.Count > 0)
                        {
                            for (int i = 0; i < carros.Count; i++)
                            {
                                int numero = Convert.ToInt32(carros[i]);

                                Carro c = new Carro(numero, null, null, null);

                                try
                                {
                                    Terminal t = gerenciadorTerminal.EnviaCarro(c, true);

                                    if (t == null)
                                    {
                                        Console.WriteLine(string.Format("Servidor - EscutaChamadaCopias - 001 : carro {0} não foi possivel ser forçado", c.Numero));
                                        //outputmessage(string.Format("Servidor - EscutaChamadaCopias - 001 : carro {0} não foi possivel ser forçado", c.Numero));
                                    }
                                    else
                                    {
                                        localhost.RemoveCarro(c);
                                    }
                                }
                                catch (TerminalComunicationException ex)
                                {
                                    Console.WriteLine(string.Format("Servidor - EscutaChamadasCopias - 002 : erro ao forçar carro {0} a copiar\n{1}\n{2}", carros[i], ex.Message, ex.StackTrace));
                                    //outputmessage(string.Format("Servidor - EscutaChamadasCopias - 002 : erro ao forçar carro {0} a copiar\n{1}\n{2}", carros[i], ex.Message, ex.StackTrace));
                                }
                                Thread.Sleep(30000);
                            }
                        }
                        Thread.Sleep(65000);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Terminal - EscutaChamadasCopias - " + ex.Message);
                    }
                }
            }).Start();
        }

        /// <summary>
        /// Registra continuamente em um arquivo os carros e seus respectivos status
        /// </summary>
        /// <param name="carros"></param>
        public static void RotinaSaidaJson(List<Carro> carros)
        {
            new Thread((ThreadStart)delegate
            {
                try
                {
                    DateTime d = DateTime.Now.AddDays(-1);
                    var data = carros.Select(c => new
                    {
                        n = c.Numero,
                        p = c.Pingando ? 1 : 0,
                        g = c.Gravando ? 1 : 0,
                        e = c.UltimoPing < d ? 1 : 0
                    });
                    int sleep_acc = 60000;
                    int sleep = 80000;
                    while (true)
                    {
                        string json = JsonConvert.SerializeObject(data);
                        CamadaDados.EscreveArquivo(@"c:\rioservice\ftp\servidor\data.json", json);
                        
                        //if (sleep_acc >= 60000)
                        //{
                        //    CamadaDados.EscreveArquivo(@"c:\rioservice\ftp\servidor\hist\" + DateTime.Now.ToString("yyyyMMddHHmm00") + ".json", json);
                        //    sleep_acc = 0;
                        //}
                        Thread.Sleep(sleep);
                        sleep_acc += sleep;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Servidor - RotinaSaidaJson : " + ex.Message);
                    //outputmessage("Servidor - RotinaSaidaJson : " + ex.Message);
                }
            }).Start();
        }

        /// <summary>
        /// Verifica continuamente se existe algum carro que precisa ser atualizado, caso exista algum, o método irá atualizar automaticamente o carro.
        /// </summary>
        public static void RotinaAtualizacaoCarros()
        {
            new Thread((ThreadStart)delegate
            {
                Console.WriteLine("Servidor - RotinaAtualizacaoCarros - Inicio");
                //outputmessage("Servidor - RotinaAtualizacaoCarros - Inicio");

                CamadaDados dados = new CamadaDados();
                while (true)
                {
                    try
                    {
                        string path = @"c:\rioservice\ftp\servidor\terminais\updates.TRM";

                        List<string> s = dados.LerArquivoTrm(path);

                        if (s.Count > 0)
                        {
                            for (int i = 0; i < s.Count; i++)
                            {
                                int id = Convert.ToInt32(s[i]);

                                carros[carros.FindIndex(a => a.Numero == id)] = controle.GetCarroByNumero(id);
                                string pathTransferencia = @"c:\rioservice\ftp\servidor\transferencias\" + id + ".json";

                                if (File.Exists(pathTransferencia))
                                    File.Delete(pathTransferencia);

                                dados.RemoveLinhaTrm(path, s[i]);
                                
                                Thread.Sleep(10000);
                            }
                        }

                        Thread.Sleep(45000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Servidor - RotinaAtualizacaoCarros - Exception 001 - " + ex.Message + ex.StackTrace);
                        //outputmessage("Servidor - RotinaAtualizacaoCarros - Exception 001 - " + ex.Message + ex.StackTrace);
                        throw new Exception("Servidor - RotinaAtualizacaoCarros : " + ex.Message);
                    }
                }
            }).Start();
        }

        public static void RestauraSistema()
        {
            try
            {
                string[] files = Directory.GetFiles(@"c:\rioservice\ftp\servidor\transferencias\");
                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Servidor - RestauraSistema - 001: " + ex.Message);
            }
        }

    }
}
