using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Data;
using ProjetoFTP.Utilidades.exceptions;

namespace ProjetoFTP.Utilidades
{
    public class GerenciadorTerminal
    {
        private List<Terminal> _terminais;
        private bool _existeComunicação;
        private bool _existiaComunicação;
        private CamadaControle _camadaControle;
        private Log execlog;

        public GerenciadorTerminal()
        {
            try
            {
                this._existeComunicação = false;
                this._existiaComunicação = true;
                this._terminais = new List<Terminal>();
                this._camadaControle = new CamadaControle();
                execlog = new Log();

                CarregaTerminais();
                CriaArquivosLeitura();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void VerificaTerminais()
        {
            bool primeiraIteracao = true;

            try
            {
                int qt = 0;
                for (int i = 0; i < _terminais.Count; i++)
                {
                    this._terminais[i].VerificaComunicação();
                    if (_terminais[i].Comunicando && _terminais[i].Habilitado)
                    {
                        qt++;
                    }
                    else
                    {
                        if (_terminais[i].TamanhoFila > 0 && !primeiraIteracao)
                        {
                            Console.WriteLine(string.Format("GerenciadorTerminal - VerificaTerminais - 001 : terminal {0} vai ser realocado", _terminais[i].Ip));
                            RealocaCarros(_terminais[i]);
                        }
                    }
                }

                this._existiaComunicação = this._existeComunicação;
                if (qt > 0)
                {
                    this._existeComunicação = true;
                }
                else this._existeComunicação = false;

                SalvaJSONTerminais();

                if (primeiraIteracao)
                    primeiraIteracao = false;

                Console.WriteLine(string.Format("GerenciadorTerminal - VerificaTerminais - 002 : {0} terminais comunicando", this._terminais.Where(t => t.Comunicando && t.Habilitado).Count()));
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("servidor.log", "GerenciadorTerminal : VerificaTerminais : " + ex.Message);
                throw new Exception("GerenciadorTerminal - Exception 001 - VerificaTerminais : " + ex.Message);
            }
        }

        public Terminal EnviaCarro(Carro carro)
        {
            List<string> carrosCrip = Terminal.Localhost.GetCarrosFila();
            for (int i = 0; i < this._terminais.Count; i++)
            {
                carrosCrip.AddRange(this._terminais[i].GetCarrosFila());
            }

            for (int i = 0; i < carrosCrip.Count; i++)
            {
                if (carrosCrip[i].Split(';')[0] == carro.Numero.ToString())
                    return null;
            }

            try
            {
                Terminal terminal = PegaTerminalOcioso();
                terminal.EnviaCarroFila(carro);
                return terminal;
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("camadacontrole.log", "GerenciadorTerminal - EnviaCarro : " + ex.Message);
                Console.WriteLine(string.Format("GerenciadorTerminal - EnviaCarro - 001 : {0}", ex.Message));
                throw new Exception("GerenciadorTerminal - Exception 002 - EnviaCarro : " + ex.Message);
            }
        }

        public Terminal EnviaCarro(Carro carro, bool forca)
        {
            try
            {
                List<string> carrosCrip = new List<string>();
                for (int i = 0; i < this._terminais.Count; i++)
                {
                    carrosCrip.AddRange(this._terminais[i].GetCarrosFila());
                }
                for (int i = 0; i < carrosCrip.Count; i++)
                {
                    if (carrosCrip[i].Split(';')[0] == carro.Numero.ToString())
                        return null;
                }
                Terminal terminal = PegaTerminalOcioso();
                terminal.EnviaCarroFila(carro);
                return terminal;
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("servidor.log", "GerenciadorTerminal - EnviaCarro : " + ex.Message);
                throw new Exception("GerenciadorTerminal - Exception 003 - EnviaCarro : " + ex.Message);
            }
        }

        private Terminal PegaTerminalOcioso()
        {
            try
            {
                List<Terminal> ligadas = null;
                ligadas = _terminais.Where(t => t.Comunicando && t.Habilitado).ToList<Terminal>();
                if (ligadas.Count() > 0)
                {
                    return ligadas.Where(t => t.TamanhoFila == ligadas.Min(t2 => t2.TamanhoFila)).First();
                }
                else
                {
                    Console.WriteLine("GerenciadorTerminal - PegaTerminalOcioso - 001 : Não há terminal comunicando");
                    throw new TerminalComunicationException("não há terminal comunicando");
                }
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("servidor.log", "GerenciadorTerminal - Exception 004 - PegaTerminalOcioso : " + ex.Message);
                throw new Exception("GerenciadorTerminal - Exception 004 - PegaTerminalOcioso : " + ex.Message);
            }
        }

        private void CarregaTerminais()
        {
            try
            {
                this._terminais = new List<Terminal>();
                DataSet ds = new CamadaDados().RealizaConsultaSql("SELECT t.IP, t.NOME_MAQUINA FROM ESTACOES t");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Terminal terminal = new Terminal(ds.Tables[0].Rows[i]["NOME_MAQUINA"].ToString(),
                                                         ds.Tables[0].Rows[i]["IP"].ToString());
                        this._terminais.Add(terminal);
                    }
                }
            }
            catch
            {
                //execlog.GravalogExec("servidor.log", "GerenciadorTerminal - CarregaTerminais - 001 : não foi possivel carregar os terminais.");
                Console.WriteLine("GerenciadorTerminal - CarregaTerminais - 001 : não foi possivel carregar os terminais.");
                throw new Exception("GerenciamentoTerminal - CarregaTerminais - 001");
            }
        }

        private void CriaArquivosLeitura()
        {
            try
            {
                foreach (Terminal terminal in this._terminais)
                {
                    string arquivo = string.Format("{0}{1}.TRM", CamadaConfiguracao.DIRETORIO_TERMINAIS, terminal.Ip.Replace('.', '-'));
                    if (!File.Exists(arquivo))
                    {
                        File.Create(arquivo);
                    }
                }
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("servidor.log", "GerenciadorTerminal - CriaArquivosLeitura : " + ex.Message);
                throw new Exception("GerenciadorTerminal - Exception 005 - CriaArquivosLeitua : " + ex.Message);
            }
        }

        private void RealocaCarros(Terminal terminalInativo)
        {
            try
            {
                List<Carro> carrosTerminal = new List<Carro>();
                this._camadaControle.GetCarrosTerminal(terminalInativo, carrosTerminal);
                int qt = carrosTerminal.Count;
                for (int i = 0; i < qt; i++)
                {
                    try
                    {
                        Carro carro = carrosTerminal.Find(c => c.Numero == carrosTerminal[i].Numero);
                        terminalInativo.RemoveCarro(carro);
                        EnviaCarro(carro);
                    }
                    catch (Exception ex)
                    {
                        //execlog.GravalogExec("servidor.log", "GerenciadorTerminal - Exception 006 - CriaArquivosLeitura : " + ex.Message);
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch
            {
                Console.WriteLine("GerenciadorTerminal - RealocaCarros : Não há terminal comunicando");
                throw new Exception("não há terminal comunicando- 002");
            }
        }

        public void AlocaCopiasForcadas(List<Carro> carros)
        {
            try
            {
                List<int> copiasForcadas = new List<int>();

                for (int i = 0; i < copiasForcadas.Count; i++)
                {
                    Carro carro = carros[carros.FindIndex(c => c.Numero == copiasForcadas[i])];
                    if (carro != null)
                    {
                        EnviaCarro(carro);
                    }
                }
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("servidor.log", "GerenciadorTerminal - Exception 007 - AlocaCopiaForcadas : " + ex.Message);
                throw new Exception("GerenciadorTerminal - Exception 007 - AlocaCopiasForcadas : " + ex.Message);
            }

        }

        public void SalvaJSONTerminais()
        {
            List<JSONTerminal> saida = new List<JSONTerminal>();
            for (int i = 0; i < _terminais.Count; i++)
            {
                saida.Add(new JSONTerminal()
                {
                    Ip = _terminais[i].Ip,
                    Nome = _terminais[i].Nome,
                    ImageUrl = (_terminais[i].Comunicando) ? "/img/terminal_ligado.png" : "/img/terminal_desligado.png"
                });
            }
            string json = JsonConvert.SerializeObject(saida);

            try
            {
                CamadaDados.EscreveArquivo(CamadaConfiguracao.JSON_LISTA_TERMINAIS, json);
            }
            catch
            {
                //execlog.GravalogExec("servidor.log", "GerenciadorTerminal - SalvaJSONTerminais - 001");
                Console.WriteLine("GerenciadorTerminal - SalvaJSONTerminais - 001");
                //SalvaJSONTerminais();
            }
        }

    }
}
