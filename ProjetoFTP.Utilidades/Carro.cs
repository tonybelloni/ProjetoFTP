using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Net;

namespace ProjetoFTP.Utilidades
{
    public enum TipoCopia { Guia = -1, UltimoArquivo = 0, Completa = 1 }
    public enum StatusCopia { Nenhum = -2, CopiaCompleta = 1, CopiaParcial = 0, CopiaIncompleta = -1 }
    public enum StatusCameras { Vazia = -1, Nenhum = 0, Baixa = 1, Parcial = 2, Completa = 3 }
    public enum TipoDvr { Desconhecido, GeoVision, mDVR }

    public class Carro
    {
        public static readonly string EXCEPTION_LIMITE_ERROS = "O PROCESSO DE CÓPIAS EXCEDEU O NUMERO LIMITE DE ERROS SUCESSIVOS";
        public int Numero { get; set; }
        public string Ip { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string UltimoArquivo {get;set;}
        public string PenDrive { get; set; }
        public TipoDvr TipoDvr { get; set; }
        public DateTime UltimaCopia { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public DateTime PeriodoInicial { get; set; }
        public DateTime PeriodoFinal { get; set; }
        public DateTime DataGuia { get; set; }
        public DateTime UltimoPing { get; set; }
        public FTP Ftp { get; set; }
        public StatusCopia Status { get; set; }
        public StatusCopia UltimoStatus { get; set; }
        public bool Copiando { get; set; }
        public bool CopiandoNoTerminal { get; set; }
        public bool Conectando { get; set; }
        public bool Copiado { get { return (UltimaCopia > (DateTime.Now - new TimeSpan(12,0,0))) ? true : false; } }
        public bool Pingando { get; set; }
        public int QuantidadeArquivosValidos { get; set; }
        public int QuantidadeArquivosTotal { get; set; }
        public int QuantidadeArquivosCopiados { get; set; }
        public long VolumeDadosTotal { get; set; }
        public long VolumeDadosCopiados { get; set; }
        public ServerResponse ServerResponse { get; set; }
        public int[] Cameras { get; set; }
        public int QuantidadeArquivosValidosPorCamera { get; set; }
        public bool Gravando { get; set; }

        private CamadaControle _camadaControle;
        private ResultadoTransferencia transferencia;
        private int qtErros = 0;
        private Log logCopia;

        public Carro(int numero, string ip, string usuario, string senha)
        {
            this.Numero = numero;
            this.Ip = ip;
            this.Usuario = usuario;
            this.Senha = senha;
            this.Status = StatusCopia.Nenhum;
            this.Ftp = new FTP(this.Ip, this.Usuario, this.Senha);
            this._camadaControle = new CamadaControle();
            this.logCopia = new Log(@"c:\rioservice\ftp\logs\copia.log");
        }
        
        public void IdentificaEquipamento()
        {
            try
            {
                //logCopia.GravalogExec("servidor.log", "Carro - GetBannerMessage");
                //string bannerMessage = this.Ftp.GetBannerMessage();

                //logCopia.GravalogExec("servidor.log", "Carro - Processando banner...");
                //if (bannerMessage != null)
                //{
                    /*string[] tokens = bannerMessage.Split(' ');
                    TipoDvr tipo = TipoDvr.Desconhecido;

                    logCopia.GravalogExec("servidor.log", "Carro - IdentificaEquipamento - 001 : " + tokens[1]);

                    switch (tokens[1])
                    {
                        case "ProFTPD":
                            tipo = TipoDvr.GeoVision;
                            break;
                        case "Rio":
                            tipo = TipoDvr.mDVR;
                            break;
                        default:
                            tipo = TipoDvr.Desconhecido;
                            break;
                    }*/
                    this.TipoDvr = TipoDvr.mDVR;
                //}
                //else
                //{
                //    this.TipoDvr = Utilidades.TipoDvr.Desconhecido;
                ////}

                //logCopia.GravalogExec("servidor.log", "Carro - IdentificaEquipamento - 002 : " + this.TipoDvr);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Carro - Identifica Equipamento : " + ex.Message);
                throw new Exception("Carro - IdentificaEquipamento : " + ex.Message);
            }
        }

        private void FiltraArquivos(ref List<string> arquivos, Int64 j)
        {
            List<string> arqsCopia = new List<string>();
            for (int i = 0; i < arquivos.Count; i++)
            {
                if ((arquivos[i].Contains(".avi") || arquivos[i].Contains(".mp4")) && (arquivos[i].Contains("event")))
                {

                    string[] tokens = arquivos[i].Split('/');
                    string fileName = tokens[tokens.Length - 1];
                    string data = fileName.Substring(fileName.LastIndexOf("event") + 5, 14);
                    Int64 aux = Convert.ToInt64(data);
                    if (aux > j)
                    {
                        string arq = arquivos[i];
                        arqsCopia.Add(arq);
                        this.VolumeDadosTotal += Convert.ToInt64(arq.Split(';')[1]);
                    }
                }
            }
            arquivos = arqsCopia;
        }

        private string FormataCaminhoArquivo(string arquivo, string caminhoPenDrive)
        {
            if (string.IsNullOrEmpty(caminhoPenDrive))
                caminhoPenDrive = this.Numero.ToString();
            string nomeArquivo = arquivo.Substring(arquivo.LastIndexOf("/") + 1);
            string diretorioArquivo = arquivo.Substring(arquivo.IndexOf('/', 3) + 1);
            diretorioArquivo = diretorioArquivo.Substring(0, diretorioArquivo.IndexOf(nomeArquivo)).Replace('/', '\\');
            CamadaDados dados = new CamadaDados();
            string pathImagens = dados.RealizaConsultaSql("SELECT PATH_IMAGENS_DIARIAS FROM PARAMETROS_SISTEMAS").Tables[0].Rows[0][0].ToString();
            string resultado = null;
            if (!arquivo.Contains(".info"))
            {
                string dataArquivo = nomeArquivo.Substring(5, nomeArquivo.Length - 5);
                string data = string.Format("{0}-{1}-{2}", new object[] { dataArquivo.Substring(0, 4), dataArquivo.Substring(4, 2), dataArquivo.Substring(6, 2) });
                resultado = string.Format(@"{0}\{1}\{2}\{3}\{4}", new object[] { pathImagens, data, caminhoPenDrive, diretorioArquivo, nomeArquivo });
            }
            else
            {
                resultado = string.Format(@"{0}\{1}\{2}\{3}", pathImagens, DateTime.Now.ToString("yyyy-MM-dd"), caminhoPenDrive, nomeArquivo);
            }
            if(!Directory.Exists(resultado.Substring(0,resultado.LastIndexOf('\\'))))
            {
                string[] paths = resultado.Substring(0,resultado.LastIndexOf('\\')).Split('\\');
                string pathFinal = null;
                for (int i = 0; i < paths.Count(); i++)
                {
                    pathFinal += paths[i] + '\\';
                    if (!Directory.Exists(pathFinal))
                        Directory.CreateDirectory(pathFinal);
                }
            }
            return resultado;
        }

        private string GetNumeroPenDrive(List<string> arquivos)
        {
            string result = null;
            for (int i = 0; i < arquivos.Count; i++)
            {
                if (arquivos[i].Contains(".LOC"))
                {
                    result = arquivos[i].Substring(arquivos[i].LastIndexOf("/") + 1,7);
                    break;
                }
            }
            return result;
        }

        private Int64 getArquivoNumero(string arquivo)
        {
            try
            {
                string[] tokens = arquivo.Split('/');
                string fileName = tokens[tokens.Length - 1];
                string data = fileName.Substring(fileName.LastIndexOf("event") + 5, 14);
                return Convert.ToInt64(data);
            }
            catch (Exception ex)
            {
                throw new Exception(" getArquivoNumero(" + arquivo + ") - " + ex.Message);
            }
        }

        private void ListaArquivos(List<string> arqs)
        {
            try
            {
                Ftp.ListFiles("", arqs);
            }
            catch
            {
                ServerResponse = CamadaConfiguracao.GetServerResponse("400");
                throw new Exception("Erro ao listar arquivos do equipamento.");
            }
        }

        private void CopiaArquivo(string arquivo, FTP ftp)
        {
            string arquivoOrigem = arquivo.Substring(0, arquivo.LastIndexOf(";"));
            
            //Console.WriteLine(arquivoOrigem);
            
            string[] tokens = arquivoOrigem.Split('/');
            string fileName = tokens[tokens.Length - 1];
            string data = fileName.Substring(fileName.LastIndexOf("event") + 5, 8);
            string camera = null;

            if (fileName.Contains(".avi"))
               camera = fileName.Substring(fileName.LastIndexOf("avi") - 4, 3);
            else if (fileName.Contains(".mp4"))
                camera = fileName.Substring(fileName.LastIndexOf("mp4") - 4, 3);

            data = data.Insert(4, "-").Insert(7, "-");

            string arquivoDestino = string.Format(@"d:\imagens\{0}\{1}\{2}\", data, this.Numero.ToString(), camera);
            
            if (!Directory.Exists(arquivoDestino))
                Directory.CreateDirectory(arquivoDestino);
            arquivoDestino += fileName;
            try
            {
                ftp.Download(arquivoOrigem, arquivoDestino);
                int size = Convert.ToInt32(arquivo.Split(';')[1]);
                this.VolumeDadosCopiados += size;
                this.QuantidadeArquivosCopiados++;
                transferencia.ArquivosCopiados++;
                transferencia.VolumeCopiado = (this.VolumeDadosCopiados / 1048576);
                double sec = (DateTime.Now - DataInicial).TotalSeconds;
                transferencia.VelocidadeMedia = (transferencia.VolumeCopiado / sec);
                UltimoArquivo = arquivoOrigem;
                qtErros = 0;
            }
            catch (Exception ex)
            {
                logCopia.AddLog(ex.Message);
                Console.WriteLine(Numero + "  - " + ex.Message);
                qtErros++;
                if (qtErros > 10)
                {
                    ServerResponse = CamadaConfiguracao.GetServerResponse("300");
                    throw new Exception(EXCEPTION_LIMITE_ERROS);
                }
            }
        }

        public TipoCopia CopiaArquivos()
        {
            if (!Ping())
            {
                ServerResponse = CamadaConfiguracao.GetServerResponse("300");
                throw new Exception("Ip não responde ao ping");
            }

            if (TipoDvr == Utilidades.TipoDvr.Desconhecido)
                this.IdentificaEquipamento();

            transferencia = new ResultadoTransferencia();
            transferencia.Numero = this.Numero;
            qtErros = 0;
            
            this.Copiando = true;
            if (Copiando)
                this._camadaControle.SetCarroCopiando(this, false);
            
            this._camadaControle.SetCarroCopiando(this, true);

            this.QuantidadeArquivosCopiados = 0;
            this.VolumeDadosCopiados = 0;
            this.VolumeDadosTotal = 0;
            
            List<string> arquivos = new List<string>();
            
            TipoCopia tipo = TipoCopia.Completa;
            
            try
            {
                Console.WriteLine(DateTime.Now.ToString() + " - (" + this.Numero + "|" + this.TipoDvr + ") - Conectando ao servidor FTP");
                //logCopia.GravalogExec("terminal.log", "Carro - (" + this.Numero + "|" + this.TipoDvr + ") - Conectando ao servidor FTP");

                this.DataInicial = DateTime.Now;

                Stopwatch sw = new Stopwatch();
                sw.Start();
                
                arquivos = new List<string>();
                ListaArquivos(arquivos);
                arquivos = arquivos.OrderBy(a => a).ToList();
                
                sw.Stop();
                
                Console.WriteLine(DateTime.Now.ToString() + " - (" + this.Numero + "|" + this.TipoDvr + ") - listagem completa [" + arquivos.Count + "arqs / " + sw.ElapsedMilliseconds + "ms]");
                //logCopia.GravalogExec("terminal.log", "Carro - (" + this.Numero + "|" + this.TipoDvr + ") - listagem completa [" + arquivos.Count + "arqs / " + sw.ElapsedMilliseconds + "ms]");

                this.QuantidadeArquivosTotal = arquivos.Count;
                
                if (this.QuantidadeArquivosTotal == 0)
                {
                    ServerResponse = CamadaConfiguracao.GetServerResponse("500");
                    logCopia.GravalogExec("terminal.log", "Carro - carro = " + this.Numero + " não contém arquivos válidos");
                    throw new Exception("Equipamento vazio");
                }

                this.PenDrive = GetNumeroPenDrive(arquivos); 
                
#region define o melhor filtro para a copia
                if (UltimoStatus == StatusCopia.CopiaCompleta)
                {
                    DateTime dataGuia = this._camadaControle.PegaDataGuia(this);
                    if (dataGuia != DateTime.MinValue)
                    {
                        DataGuia = dataGuia;
                        tipo = TipoCopia.Guia;
                        this.PeriodoFinal = DateTime.Now;
                        this.PeriodoInicial = dataGuia;
                        FiltraArquivos(ref arquivos, Convert.ToInt64(dataGuia.ToString("yyyyMMddHHmmss")));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(UltimoArquivo))
                        {
                            tipo = TipoCopia.UltimoArquivo;
                            string dataUltimoArquivo = UltimoArquivo.Substring(UltimoArquivo.LastIndexOf('/') + 1);
                            dataUltimoArquivo = dataUltimoArquivo.Substring(0, dataUltimoArquivo.LastIndexOf('.'));
                            FiltraArquivos(ref arquivos, this.getArquivoNumero(UltimoArquivo));
                        }
                        else
                        {
                            tipo = TipoCopia.Completa;
                            this.PeriodoFinal = DateTime.Now;
                            this.PeriodoInicial = DateTime.Now.AddDays(-1);
                            FiltraArquivos(ref arquivos, Convert.ToInt64(DateTime.Now.Subtract(new TimeSpan(1,0,0,0)).ToString("yyyyMMddHHmmss")));
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(UltimoArquivo))
                    {
                        tipo = TipoCopia.UltimoArquivo;
                        this.PeriodoFinal = DateTime.Now;
                        FiltraArquivos(ref arquivos, this.getArquivoNumero(UltimoArquivo));
                    }
                    else
                    {
                        tipo = TipoCopia.Completa;
                        this.PeriodoFinal = DateTime.Now;
                        this.PeriodoInicial = DateTime.Now.AddDays(-1);
                        FiltraArquivos(ref arquivos, Convert.ToInt64(DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)).ToString("yyyyMMddHHmmss")));
                    }
                }
#endregion

                Console.WriteLine(DateTime.Now.ToString() + " - (" + this.Numero + "|" + this.TipoDvr + ") - filtragem completa. Arquivos: " + arquivos.Count);
                //logCopia.GravalogExec("terminal.log", "Carro - (" + this.Numero + "|" + this.TipoDvr + ") - filtragem completa. Arquivos: " + arquivos.Count);

                if (arquivos.Count > 0)
                {
                    transferencia.ArquivosTotal = arquivos.Count;
                    transferencia.VolumeTotal = (this.VolumeDadosTotal / 1048576);
                    transferencia.DataInicial = this.DataInicial.ToString();

                    #region ordena e captura intervalo de arquivos
                    arquivos = arquivos.Where(a => (a.Contains(".avi") || a.Contains(".mp4")) && a.Contains("event")).OrderBy(a => a.Split(';')[0].Substring(a.LastIndexOf('/') + 1).Substring(0, 19)).ToList();
                    string dtIni = arquivos[arquivos.Count - 1];
                    dtIni = dtIni.Substring(dtIni.LastIndexOf('/') + 1);
                    dtIni = dtIni.Substring(0, dtIni.LastIndexOf('.'));
                    this.PeriodoInicial = new DateTime(Convert.ToInt32(dtIni.Substring(0 + 5, 4)),
                                                       Convert.ToInt32(dtIni.Substring(4 + 5, 2)),
                                                       Convert.ToInt32(dtIni.Substring(6 + 5, 2)),
                                                       Convert.ToInt32(dtIni.Substring(8 + 5, 2)),
                                                       Convert.ToInt32(dtIni.Substring(10 + 5, 2)),
                                                       Convert.ToInt32(dtIni.Substring(12 + 5, 2)));
                    string dtFinal = arquivos[0];
                    dtFinal = dtFinal.Substring(dtFinal.LastIndexOf('/') + 1);
                    dtFinal = dtFinal.Substring(0, dtFinal.LastIndexOf('.'));
                    this.PeriodoFinal = new DateTime(Convert.ToInt32(dtFinal.Substring(0 + 5, 4)),
                                                     Convert.ToInt32(dtFinal.Substring(4 + 5, 2)),
                                                     Convert.ToInt32(dtFinal.Substring(6 + 5, 2)),
                                                     Convert.ToInt32(dtFinal.Substring(8 + 5, 2)),
                                                     Convert.ToInt32(dtFinal.Substring(10 + 5, 2)),
                                                     Convert.ToInt32(dtFinal.Substring(12 + 5, 2)));
                    #endregion

                    this.QuantidadeArquivosValidos = arquivos.Count;

                    #region thread para salvar status da copia
                    new Thread((ThreadStart)delegate
                    {
                        while (this.Copiando)
                        {
                            try
                            {
                                Transferencia.Atualiza(transferencia);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("ERRO - Carro - 003 - Atualiza Dados Transferencia : " + ex.Message + " : " + ex.StackTrace.ToString());
                            }
                            Thread.Sleep(2000);
                        }
                    }).Start();

                    #endregion

                    int baldesLengthMax = 1;
                    int baldesLength = arquivos.Count < baldesLengthMax ? arquivos.Count : baldesLengthMax;
                    int baldesCapacity = (int)Math.Ceiling(arquivos.Count / (double)baldesLength);
                    string[][] baldes = new string[baldesLength][];
                    int aux = (baldesCapacity * baldesLength) - arquivos.Count;
                    for (int i = 0; i < baldesLength; i++)
                    {
                        int j = (aux == 0) ? baldesCapacity : (i == baldesLength - 1) ? baldesCapacity - aux : baldesCapacity;
                        baldes[i] = arquivos.GetRange(i * baldesCapacity, j).ToArray();
                    }
                    if (baldes.Length > 0)
                    {
                        Console.WriteLine(DateTime.Now.ToString() + " - (" + this.Numero + "|" + this.TipoDvr + ") - transferência iniciada.");
                        //logCopia.GravalogExec("terminal.log", "Carro - (" + this.Numero + "|" + this.TipoDvr + ") - transferência iniciada.");

                        for (int j = 0; j < baldes[0].Length; j++)
                        {
                            CopiaArquivo(baldes[0][j], this.Ftp);
                            _camadaControle.AtualizaUltimoArquivo(this);
                        }
                    }

                    #region define status da copia
                    if (this.QuantidadeArquivosCopiados >= arquivos.Count * 0.8)
                    {
                        Status = StatusCopia.CopiaCompleta;
                        ServerResponse = CamadaConfiguracao.GetServerResponse("200");
                    }
                    else
                    {
                        Status = StatusCopia.CopiaParcial;
                        ServerResponse = CamadaConfiguracao.GetServerResponse("200");
                        if (this.QuantidadeArquivosCopiados == 0)
                            throw new Exception("nenhum arquivo foi copiado");
                    }
                    #endregion
                }
                else
                {
                    Console.WriteLine("Carro = " + this.Numero.ToString() + " - Nenhum arquivo para copia");
                    //logCopia.GravalogExec("terminal.log", "Carro = " + this.Numero.ToString() + " - Nenhum arquivo para copia");
                }
            }
            catch (Exception ex)
            {
                logCopia.AddLog(ex.Message);
                Console.WriteLine(ex);
                Status = StatusCopia.CopiaIncompleta;
                if (ServerResponse == null)
                    ServerResponse = CamadaConfiguracao.GetServerResponse("1000");
            }
            finally
            {
                Transferencia.Remove(transferencia);
                this.UltimaCopia = DateTime.Now;
                this.DataFinal = this.UltimaCopia;
                this._camadaControle.SetCarroCopiando(this, false);
                Console.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}", new object[] { Numero, DataInicial, DataFinal, (DataFinal - DataInicial).TotalMinutes.ToString("0.0"), arquivos.Count, QuantidadeArquivosCopiados, VolumeDadosTotal / 1000000, VolumeDadosCopiados / 1000000, (int)tipo }));
                this.Copiando = false;
            }
            return tipo;
        }

        public void TransfereArquivos()
        {
            List<string> arqs = new List<string>();
            Ftp.ListFiles("", arqs);
            if(this.TipoDvr == Utilidades.TipoDvr.GeoVision)
                this.PenDrive = GetNumeroPenDrive(arqs);
            string root = (string.IsNullOrEmpty(this.PenDrive)) ? @"c:\imagens\" + this.Numero + @"\" : @"c:\imagens\" + this.PenDrive + @"\";
        }

        public void VerificaCameras()
        {
            try
            {

                Log logexec;
                logexec = new Log();

                Stopwatch sw = new Stopwatch();
                sw.Start();
                this.DataGuia = this._camadaControle.PegaDataGuia(this);
                DateTime dtin = DateTime.Now - new TimeSpan(0, 59, 0);
                DateTime dtfn = DateTime.Now - new TimeSpan(0, 1, 0);
                string datainicial = dtin.ToString("yyyyMMddHHmm00");
                string datafinal = dtfn.ToString("yyyyMMddHHmm00");

                if (TipoDvr == Utilidades.TipoDvr.mDVR)
                {
                    /* Console.WriteLine("etapa1");
                     HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format("http://{0}:7777/cgi-bin/arqscamera.cgi?datainicial={1}&datafinal={2}", this.Ip, datainicial, datafinal));
                     Console.WriteLine(string.Format("http://{0}:7777/cgi-bin/arqscamera.cgi?datainicial={1}&datafinal={2}", this.Ip, datainicial, datafinal));
                     Console.WriteLine("etapa2");
                     request.Timeout = 30000;
                     request.ReadWriteTimeout = 30000;
                     request.KeepAlive = false;
                     WebResponse response = request.GetResponse();
                     Console.WriteLine("etapa3");
                     StreamReader sr = new StreamReader(response.GetResponseStream());
                     Console.WriteLine("etapa4");

                     string s = null;
                     char[] buffer = new char[1024];
                     int aux = sr.Read(buffer, 0, 1024);
                     s += new string(buffer);

                     while (aux > 0)
                     {
                         buffer = new char[1024];
                         aux = sr.Read(buffer, 0, 1024);
                         s += new string(buffer);
                     }

                     sr.Close();
                     sr.Dispose();
                     response.Close();
                     request = null;
                     buffer = null;
                     s = s.Replace("\0", string.Empty);
                     s = s.TrimEnd('\n');

                     string[] tokens = s.Split(';');
                     Cameras = new int[tokens.Length];
                     bool gravando = true;

                     for (int i = 0; i < Cameras.Length; i++)
                     {
                         Cameras[i] = Convert.ToInt32(tokens[i]);
                         if (Cameras[i] == 0)
                             gravando = false;
                     }

                     tokens = null;
                     this.Gravando = gravando;*/
                    this.Gravando = true;

                    if (this.Gravando)
                        Console.WriteLine("Carro : " + this.Numero + " : Gravando");
                    else
                        Console.WriteLine("Carro : " + this.Numero + " : NAO ESTA GRAVANDO");

                }
                else
                {
                    this.Cameras = new int[] { 0, 0 };
                    this.QuantidadeArquivosTotal = 0;
                    this.QuantidadeArquivosValidos = 0;
                    this.QuantidadeArquivosValidosPorCamera = 0;
                    this.Conectando = false;
                    List<string> arquivos = new List<string>();
                    Ftp.ListFiles("", arquivos);
                    this.Conectando = true;
                    arquivos = arquivos.Where(a => a.Contains(datainicial.Substring(0, 12))).ToList();
                    if (arquivos.Count > 0)
                        this.Gravando = true;
                    else
                        this.Gravando = false;
                }

                sw.Stop();

                //logexec.GravalogExec("servidor.log", string.Format("{0} {1} verificado em {2}ms", Numero, TipoDvr.ToString(), sw.ElapsedMilliseconds));
                Console.WriteLine(string.Format("{0} - {1} {2} verificado em {3}ms", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Numero, TipoDvr.ToString(), sw.ElapsedMilliseconds));
            }
            catch (WebException ex)
            {
                this.Gravando = false;
                Console.WriteLine("Carro - Erro ao verificar camera : " + ex.Message);
            }
            catch (Exception ex)
            {
                this.Gravando = false;
                Console.WriteLine(string.Format("{0} - {1} {2} erro ao verificar câmeras", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Numero, TipoDvr.ToString()));
                throw new Exception("Carro - VerificaCameras : " + ex.Message);
            }
        }

        public bool Ping()
        {
            Ping png = new Ping();
            PingReply pr;

            logCopia.GravalogExec("terminal.log", string.Format("Carro - Ping : Inicio ping - IP : {0}", Ip));

            Pingando = false;

            try
            {
               pr = png.Send(Ip, 3000);
               if (pr.Status == IPStatus.Success)
               {
                   Pingando = true;
               }

               return Pingando;
            }
            catch (Exception ex)
            {
                //logCopia.GravalogExec("servidor.log", "Carro - Ping : " + ex.Message + ex.StackTrace);
                throw new Exception("Carro - Ping : " + ex.Message);
            }
        }

        public bool TemArquivos()
        {
            try
            {
                List<string> arqs = new List<string>();
                ListaArquivos(arqs);
                if (arqs.Count > 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

    }
}
