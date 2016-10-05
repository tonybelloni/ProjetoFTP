using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Data;
using System.Net;
using System.Web;

namespace ProjetoFTP.Utilidades
{
    public class CamadaControle
    {
        private CamadaDados _camadaDados;
        private Log execlog;

        public CamadaControle()
        {
            this._camadaDados = new CamadaDados();
            this.execlog = new Log();       
        }

        public List<Carro> GetCarros()
        {
            try
            {
                List<Carro> resultado = new List<Carro>();
                DataSet ds = this._camadaDados.RealizaConsultaSql("SELECT c.ID_CARRO, c.IP, c.USUARIO, c.SENHA, c.ULTIMA_COPIA, c.COPIANDO, " +
                                                                  "c.ULTIMO_ARQUIVO_COPIADO ,c.FLAG_ULTIMA_COPIA, c.ULTIMA_VERIFICACAO FROM CARROS c " +
                                                                  "WHERE c.IP IS NOT NULL ORDER BY c.ID_CARRO ASC ");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                    Carro carro = new Carro(Convert.ToInt32(ds.Tables[0].Rows[i]["ID_CARRO"]),
                                            ds.Tables[0].Rows[i]["IP"].ToString(),
                                            ds.Tables[0].Rows[i]["USUARIO"].ToString(),
                                            ds.Tables[0].Rows[i]["SENHA"].ToString());
                    carro.UltimaCopia = (ds.Tables[0].Rows[i]["ULTIMA_COPIA"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(ds.Tables[0].Rows[i]["ULTIMA_COPIA"]);
                    carro.UltimoStatus = (StatusCopia)(Convert.ToInt32(ds.Tables[0].Rows[i]["FLAG_ULTIMA_COPIA"]));
                    carro.UltimoArquivo = ds.Tables[0].Rows[i]["ULTIMO_ARQUIVO_COPIADO"].ToString();
                    carro.Copiando = (Convert.ToInt32(ds.Tables[0].Rows[i]["COPIANDO"]) == 0) ? false : true;
                    carro.UltimoPing = (ds.Tables[0].Rows[i]["ULTIMA_VERIFICACAO"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(ds.Tables[0].Rows[i]["ULTIMA_VERIFICACAO"]);
                    resultado.Add(carro);
                    sw.Stop();

                    Console.WriteLine(string.Format("{0} - {2} {1} inicializado [{3}ms]", DateTime.Now, carro.Numero, carro.TipoDvr, sw.ElapsedMilliseconds));
                    //execlog.GravalogExec("camadacontrole.log", string.Format("CamadaControle - GetCarros - {1} {0} inicializado [{2}ms]", carro.Numero, carro.TipoDvr, sw.ElapsedMilliseconds));
                }
                return resultado;
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "CamadaControle - GetCarros : " + ex.Message);
                throw new Exception("CamadaControle - GetCarros - 001 : " + ex.Message + ex.StackTrace);
            }
            
        }

        public void GetCarrosTerminal(Terminal terminal, List<Carro> carros)
        {
            //List<string> numeros = _camadaDados.LerArquivoTrm(string.Format("{0}{1}.{2}",CamadaConfiguracao.COMP_DIRETORIO_TERMINAIS,terminal.Ip.Replace('.','-'),CamadaConfiguracao.EXTENSAO_TERMINAL));
            try
            {
                List<string> l = terminal.GetCarrosFila();
                if (l.Count > 0)
                {
                        for (int i = 0; i < l.Count; i++)
                        {
                            try
                            {
                                Carro carro = carros.Find(c => c.Numero == Convert.ToInt32(l[i]));
                                if (carro == null) throw new Exception();
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    Carro carro = GetCarroByNumero(Convert.ToInt32(l[i]));
                                    carros.Add(carro);
                                }
                                catch (Exception ex0)
                                {
                                    //execlog.GravalogExec("camadacontrole.log", "CamadaControle - GetCarrosTerminal - ERRO 001 : " + ex.Message + " - " + ex.StackTrace + " - " + ex.TargetSite);
                                    Console.WriteLine(ex0.Message);
                                }
                            }
                    }
                }
                else
                {
                    Console.WriteLine("CamadaControle - GetCarrosTerminal : NÃO HAVIA CARRO NA FILA !!!");
                    //execlog.GravalogExec("camadacontrole.log", "CamadaControle - GetCarrosTerminal : NÃO HAVIA CARRO NA FILA !!!");
                }
            }
            catch(Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "CamadaControle - GetCarrosTerminal : " + ex.Message + " - " + ex.StackTrace + " - " + ex.TargetSite);
                throw new Exception("CamadaControle - GetCarros - 002 : " + ex.Message + ex.StackTrace);
            }
        }

        public void RemoveCarroLista(string caminhoTrm, Carro carro)
        {
            try
            {
                this._camadaDados.RemoveLinhaTrm(caminhoTrm, carro.Numero.ToString());
            }
            catch(Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "CamadaControle - RemoveCarroLista : " + ex.Message);
                throw new Exception("CamadaControle - GetCarros - RemoveCarroLista : " + ex.Message + ex.StackTrace);
            }
        }

        public Carro GetCarroByNumero(int numero)
        {
            if (this._camadaDados == null) this._camadaDados = new CamadaDados();
            try
            {
                DataSet resultado = this._camadaDados.RealizaConsultaSql(string.Format("SELECT c.IP, c.USUARIO, c.SENHA, c.COPIANDO, " + 
                                                                                       "c.ULTIMA_COPIA, c.FLAG_ULTIMA_COPIA," + 
                                                                                       "c.ULTIMO_ARQUIVO_COPIADO FROM CARROS c WHERE ID_CARRO = {0}", numero));
                string ip = resultado.Tables[0].Rows[0]["IP"].ToString();
                string usuario = resultado.Tables[0].Rows[0]["USUARIO"].ToString();
                string senha = resultado.Tables[0].Rows[0]["SENHA"].ToString();
                string ultimoArquivo = resultado.Tables[0].Rows[0]["ULTIMO_ARQUIVO_COPIADO"].ToString();
                DateTime ultimaCopia = (resultado.Tables[0].Rows[0]["ULTIMA_COPIA"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(resultado.Tables[0].Rows[0]["ULTIMA_COPIA"].ToString());
                Carro carro = new Carro(numero, ip, usuario, senha);
                StatusCopia ultimoStatus = (StatusCopia)(Convert.ToInt32(resultado.Tables[0].Rows[0]["FLAG_ULTIMA_COPIA"]));
                carro.UltimaCopia = ultimaCopia;
                carro.UltimoStatus = ultimoStatus;
                carro.UltimoArquivo = ultimoArquivo;
                carro.Copiando = (Convert.ToInt32(resultado.Tables[0].Rows[0]["COPIANDO"]) == 0) ? false : true;
                return carro;
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "CamadaControle - GetCarroByNumero : " + ex.Message);
                throw new Exception("CamadaControle - GetCarros - GetCarroByNumero : " + ex.Message + ex.StackTrace);
                //return GetCarroByNumero(numero);
            }
        }

        public List<Terminal> GetTerminais()
        {
            try
            {
                List<Terminal> resultado = new List<Terminal>();
                DataSet ds = this._camadaDados.RealizaConsultaSql("SELECT t.IP, t.NOME_MAQUINA FROM ESTACOES t");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Terminal terminal = new Terminal(ds.Tables[0].Rows[i]["NOME_MAQUINA"].ToString(),
                                                         ds.Tables[0].Rows[i]["IP"].ToString());
                        resultado.Add(terminal);
                    }
                }
                return resultado;
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "CamadaControle - GetTerminais : " + ex.Message);
                throw new Exception("CamadaControle - GetCarros - GetTerminais : " + ex.Message + ex.StackTrace);
            }
        }

        public Terminal GetTerminalByIp(string ip)
        {
            try
            {
                DataSet ds = this._camadaDados.RealizaConsultaSql(string.Format("SELECT t.NOME_MAQUINA, t.IP FROM ESTACOES t WHERE t.IP='{0}'", ip));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return new Terminal(ds.Tables[0].Rows[0]["NOME_MAQUINA"].ToString(), ds.Tables[0].Rows[0]["IP"].ToString());
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "CamadaControle - GetTerminalByIp : " + ex.Message);
                throw new Exception("CamadaControle - GetTerminalByIp : " + ex.Message + ex.StackTrace);
                //return null;
            }
        }

        public DateTime PegaDataGuia(Carro carro)
        {
            try
            {
                DataSet ds = this._camadaDados.RealizaConsultaSql(string.Format("select to_char(PEGADA,'dd/mm/yyyy HH24:mi:ss')," +
                                                                                "turno from interface_integracao where id_carro = {0} and trunc(pegada) = (select max(trunc(pegada))" +
                                                                                "from interface_integracao where id_carro = {0}) order by pegada", carro.Numero));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DateTime guia = DateTime.Parse(ds.Tables[0].Rows[0][0].ToString());
                    Log.AdicionaEvento(@"c:\rioservice\ftp\logs\guia.logs", string.Format("{0} - O carro {1} entrou na garagem às {2}", DateTime.Now.ToString(), carro.Numero, guia.ToString("dd/MM/yyyy HH:mm:ss")), false);
                    return guia;
                }
                else
                {
                    Log.AdicionaEvento(@"c:\rioservice\ftp\logs\guia.logs", string.Format("{1} - {0} {2}", "Não foi encontrada a guia do carro", DateTime.Now.ToString(), carro.Numero), false);
                    return DateTime.MinValue;
                }
            }
            catch(Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "CamadaControle - PegaDataGuia : " + ex.Message);
                Log.AdicionaEvento(@"c:\rioservice\ftp\logs\guia.logs", string.Format("{1} - {0}", ex.Message, DateTime.Now.ToString()), false);
                return DateTime.MinValue;
            }
        }

        public void RegistraLogCopia(object[] args)
        {
            try
            {
                this._camadaDados.RealizaConsultaSqlVoid(string.Format("INSERT INTO COPIAS_HISTORICOS(ID_COPIA, DATA_INICIO_COPIA, DATA_FIM_COPIA, NUMERO_ARQUIVOS_TOTAL, NUMERO_ARQUIVOS_COPIADOS, TAMANHO_ARQUIVOS_TOTAL, TAMANHO_ARQUIVOS_COPIADOS, ID_CARRO, TIPO_COPIA, CONEXAO_FTP,COD_EQUIPAMENTO,NUMERO_ARQUIVOS_VALIDOS, PERIODO_INICIAL, PERIODO_FINAL, CODIGO_RESULTADO, ID_USUARIO, IP_ESTACAO) " +
                                                                       "VALUES( COPIAS_HISTORICOS_SEQ.nextval, to_date('{0}','dd/mm/yyyy hh24:mi:ss'), to_date('{1}','dd/mm/yyyy hh24:mi:ss'), {2}, {3}, {4}, {5}, {6}, {7}, {8},{9},{10},to_date('{11}','dd/mm/yyyy hh24:mi:ss'),to_date('{12}','dd/mm/yyyy hh24:mi:ss'),'{13}',{14},'{15}')", args));
            }
            catch(Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "CamadaControle - RegistraLogCopia : " + ex.Message);
                throw new Exception("CamadaControle - RegistraLogCopia : " + ex.Message + ex.StackTrace);
            }
        }

        public void AtualizaInformações(Carro carro)
        {
            try
            {
                this._camadaDados.RealizaConsultaSqlVoid(string.Format("UPDATE CARROS c SET c.ULTIMO_ARQUIVO_COPIADO = '{0}', c.ULTIMA_COPIA = to_date('{1}','dd/mm/yyyy hh24:mi:ss'), c.FLAG_ULTIMA_COPIA = {2}  WHERE c.ID_CARRO = {3} ", new object[] { carro.UltimoArquivo, carro.UltimaCopia, (int)carro.Status, carro.Numero }));
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "CamadaControle - AtualizaInformacoes : " + ex.Message);
                throw new Exception("CamadaControle - AtualizaInformacoes : " + ex.Message + ex.StackTrace);
                //AtualizaInformações(carro);
            }
        }

        public void AtualizaUltimoArquivo(Carro carro)
        {
            try
            {
                this._camadaDados.RealizaConsultaSqlVoid(string.Format("UPDATE CARROS c SET c.ULTIMO_ARQUIVO_COPIADO = '{0}' WHERE c.ID_CARRO = {1} ", new object[] { carro.UltimoArquivo, carro.Numero }));
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "CamadaControle - AtualizaInformacoes : " + ex.Message);
                throw new Exception("CamadaControle - AtualizaUltimoArquivo : " + ex.Message + ex.StackTrace);
                //AtualizaInformações(carro);
            }
        }

        public void AtualizaUltimoPing(Carro carro)
        {
            try
            {
                this._camadaDados.RealizaConsultaSqlVoid(string.Format("UPDATE CARROS C SET C.ULTIMA_VERIFICACAO = to_date('{0}','dd/mm/yyyy hh24:mi:ss') WHERE C.ID_CARRO = {1}", carro.UltimoPing, carro.Numero));
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "CamadaControle - AtualizaUltimoPing : " + ex.Message);
                throw new Exception("CamadaControle - AtualizaUltimoPing : " + ex.Message + ex.StackTrace);
            }
        }

        public List<int> PegaCarrosDesatualizados()
        {
            try
            {
                List<string> trm = this._camadaDados.LerArquivoTrm(CamadaConfiguracao.DIRETORIO_TERMINAIS + "cnl_upd.TRM");
                List<int> numeros = new List<int>();
                for (int i = 0; i < trm.Count; i++)
                {
                    numeros.Add(Convert.ToInt32(Criptografia.Decrypt(trm[i], true)));
                }
                return numeros;
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "CamadaControle - PegaCarrosDesatualizados : " + ex.Message);
                throw new Exception("CamadaControle - PegaCarrosDesatualizados : " + ex.Message + ex.StackTrace);
            }
        }
        
        public List<string> PegaCarrosForcados()
        {
            try
            {
                List<string> trm = this._camadaDados.LerArquivoTrm(CamadaConfiguracao.DIRETORIO_TERMINAIS + "localhost.TRM");
                List<string> numeros = new List<string>();
                for (int i = 0; i < trm.Count; i++)
                {
                    numeros.Add(Criptografia.Decrypt(trm[i], true));
                }
                return numeros;
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "CamadaControle - PegaCarrosForcados : " + ex.Message);
                throw new Exception("CamadaControle - PegaCarrosForcados : " + ex.Message + ex.StackTrace);
            }
        }

        public void ForcaCopia(int carro)
        {
            try
            {
                Terminal localhost = Terminal.Localhost;
                List<string> carros = localhost.GetCarrosFila();
                List<Terminal> terminais = GetTerminais();
                for (int i = 0; i < terminais.Count; i++)
                {
                    carros.AddRange(terminais[i].GetCarrosFila());
                }
                string id = carro.ToString();
                for (int i = 0; i < carros.Count; i++)
                {
                    if (id == carros[i])
                        throw new Exception("O carro já está alocado em um terminal");
                }
                localhost.EnviarCarroFila(carro);
                //new Log(CamadaConfiguracao.DIRETORIO_LOGS_COPIAS + "copias_forcadas.log").AdicionaEvento(string.Format("{0} - Carro {1} foi forçado a copiar", DateTime.Now, carro), false);
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "CamadaControle - ForcaCopia : " + ex.Message);
                throw new Exception("CamadaControle - ForcaCopia : " + ex.Message + ex.StackTrace);
            }
        }

        public Version GetVersion()
        {
            try
            {
                return Version.Parse(File.ReadAllLines(CamadaConfiguracao.ARQUIVO_VERSAO)[0]);
            }
            catch
            {
                return new Version(1, 0, 0, 0);
            }
        }

        public void SetCarroCopiando(Carro carro, bool value)
        {
            try
            {
                int aux = (value) ? 1 : 0;
                this._camadaDados.RealizaConsultaSqlVoid(string.Format("UPDATE CARROS SET COPIANDO = {0} WHERE ID_CARRO = {1}", aux, carro.Numero));
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "CamadaControle - SetCarroCopiando : " + ex.Message);
                throw new Exception("CamadaControle - SetCarroCopiando : " + ex.Message + ex.StackTrace);
            }
        }
    }
}
