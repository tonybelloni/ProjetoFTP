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

                Console.Title = "Servidor FTP - Versão 2.3.1";

                Console.WriteLine("Servidor - Início - Versão : 2.3.1");
                //outputmessage("Servidor - Início - Versão : 2.3.1");

                Console.WriteLine("Servidor - RestauraSistema - " + DateTime.Now.ToString());
                //outputmessage("Servidor - RestauraSistema");
                RestauraSistema();

                Console.WriteLine("Servidor - Inicializando terminais - " + DateTime.Now.ToString());
                gerenciadorTerminal = new GerenciadorTerminal();
                controle = new CamadaControle();
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                carros = controle.GetCarros();

                while (true)
                {
                    Console.WriteLine("Servidor - RotinaAtualizacaoCarros - " + DateTime.Now.ToString());
                    //outputmessage("Servidor - RotinaAtualizacaoCarros");
                    RotinaAtualizacaoCarros();

                    Console.WriteLine("Servidor - EscutaChamadasCopias - " + DateTime.Now.ToString());
                    //outputmessage("Servidor - EscutaChamadasCopias");
                    EscutaChamadasCopias();

                    Console.WriteLine("Servidor - VerificaTerminais - " + DateTime.Now.ToString());
                    //outputmessage("Servidor - VerificaTerminais");
                    gerenciadorTerminal.VerificaTerminais();

                    Console.WriteLine("Servidor - Inicio loop principal - " + DateTime.Now.ToString());
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

                    Console.WriteLine("Servidor - RotinaSaidaJson - " + DateTime.Now.ToString());
                    RotinaSaidaJson(carros);

                    Console.WriteLine("Servidor - Fim loop principal - Aguardando nova execuçao - " + DateTime.Now.ToString());
                    Thread.Sleep(60000);
                }
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

                //outputmessage("Servidor - IniciaRotinaVerificacao - Inicio loop principal");
                try
                {
                    //outputmessage("Servidor - IniciaRotinaVerificacao - Inicio ping...");
                    if (carros[i].Ping())
                    {

                        Console.WriteLine(string.Format("Servidor - IniciaRotinaVerificacao - Carro {0} pingado - {1}", carros[i].Ip, DateTime.Now.ToString()));
                        //outputmessage(string.Format("Servidor - IniciaRotinaVerificacao - Carro {0} pingado", carros[i].Ip));

                        carros[i].UltimoPing = DateTime.Now;

                        //outputmessage("Servidor - Atualizando dados do carro...");
                        #region atualiza dados do carro

                        //if (sleepUpdate >= maxUpdateSleep)
                        //{
                            controle.AtualizaUltimoPing(carros[i]);
                            Console.WriteLine(string.Format("Servidor - IniciaRotinaVerificacao : Carro {0} - Ultima Verificacao Atualizada - {1}", carros[i].Numero, DateTime.Now.ToString()));
                            //outputmessage(string.Format("Servidor - IniciaRotinaVerificacao - 001 : O carro {0} foi atualizado", carros[i].Numero));
                        //    sleepUpdate = 0;
                        //}
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

                        Console.WriteLine("Servidor - Notificando status do FTP - " + DateTime.Now.ToString());
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

                        Console.WriteLine("Servidor - Verificando Cameras - " + DateTime.Now.ToString());
                        //outputmessage("Servidor - Verificando Cameras...");
                        carros[i].VerificaCameras();

                        lock (gerenciadorTerminal)
                        {
                            Console.WriteLine("Servidor - Enviando carro para terminal - " + carros[i].Numero.ToString() + " - " + DateTime.Now.ToString());
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
                                            Console.WriteLine("Carro " + carros[i].Numero + " já está sendo copiado em algum terminal !!! - " + DateTime.Now.ToString());
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
                                Console.WriteLine("Carro " + carros[i].Numero + " Já foi copiado nas últimas 6 horas !!! - " + DateTime.Now.ToString());
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
                        Console.WriteLine(string.Format("Servidor - IniciaRotinaVerificacao - Carro {0} não encontrado - {1}", carros[i].Ip, DateTime.Now.ToString()));
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
                }
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
            Terminal localhost = Terminal.Localhost;

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
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Terminal - EscutaChamadasCopias - " + ex.Message);
            }
        }

        /// <summary>
        /// Registra continuamente em um arquivo os carros e seus respectivos status
        /// </summary>
        /// <param name="carros"></param>
        /// 
        public static void RotinaSaidaJson(List<Carro> carros)
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

                string json = JsonConvert.SerializeObject(data);
                CamadaDados.EscreveArquivo(@"c:\rioservice\ftp\servidor\data.json", json);
                            }
            catch (Exception ex)
            {
                Console.WriteLine("Servidor - RotinaSaidaJson : " + ex.Message);
                //outputmessage("Servidor - RotinaSaidaJson : " + ex.Message);
            }
        }

        /// <summary>
        /// Verifica continuamente se existe algum carro que precisa ser atualizado, caso exista algum, o método irá atualizar automaticamente o carro.
        /// </summary>
        public static void RotinaAtualizacaoCarros()
        {
            Console.WriteLine("Servidor - RotinaAtualizacaoCarros - Inicio - " + DateTime.Now.ToString());
            //outputmessage("Servidor - RotinaAtualizacaoCarros - Inicio");

            CamadaDados dados = new CamadaDados();
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Servidor - RotinaAtualizacaoCarros - Exception 001 - " + ex.Message + ex.StackTrace);
                //outputmessage("Servidor - RotinaAtualizacaoCarros - Exception 001 - " + ex.Message + ex.StackTrace);
                throw new Exception("Servidor - RotinaAtualizacaoCarros : " + ex.Message);
            }
        }

        public static void RestauraSistema()
        {
            try
            {
                if (!Directory.Exists(@"c:\rioservice"))
                    Directory.CreateDirectory(@"c:\rioservice");

                if (!Directory.Exists(@"c:\rioservice\ftp"))
                    Directory.CreateDirectory(@"c:\rioservice\ftp");

                if (!Directory.Exists(@"c:\rioservice\ftp\servidor"))
                    Directory.CreateDirectory(@"c:\rioservice\ftp\servidor");

                if (!Directory.Exists(@"c:\rioservice\ftp\servidor\trmjson"))
                    Directory.CreateDirectory(@"c:\rioservice\ftp\servidor\trmjson");

                if (!Directory.Exists(@"c:\rioservice\ftp\servidor\notificacoes"))
                    Directory.CreateDirectory(@"c:\rioservice\ftp\servidor\notificacoes");

                if (!Directory.Exists(@"c:\rioservice\ftp\servidor\terminais"))
                    Directory.CreateDirectory(@"c:\rioservice\ftp\servidor\terminais");

                if (!Directory.Exists(@"c:\rioservice\ftp\servidor\transferencias"))
                    Directory.CreateDirectory(@"c:\rioservice\ftp\servidor\transferencias");

                if (!Directory.Exists(@"c:\rioservice\ftp\servidor\logs"))
                    Directory.CreateDirectory(@"c:\rioservice\ftp\servidor\logs");
            }
            catch (Exception ex)
            {
                throw new Exception("Servidor - Restaura Sistema - 001 : Erro ao criar diretórios da aplicação");
            }

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
                throw new Exception("Servidor - RestauraSistema - 002: " + ex.Message);
            }
        }

    }
}
