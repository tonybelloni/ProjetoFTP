using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace ProjetoFTP.Utilidades
{
    public class CamadaConfiguracao
    {

        #region REQUISITOS_GERENCIAMENTO

        public static readonly string PAGINA_TITULO_PADRAO = "Busvision Gateway Server - Rio Service Company";
        public static readonly string PERMISSAO_ADMINISTRADOR = "ADMINISTRADOR";
        public static readonly string PERMISSAO_VISUALIZADOR = "VISUALIZADOR";
        public static readonly string USUARIO_MASTER = "master";
        public static readonly string SENHA_MASTER = "rsco@rserviceprod";

        #endregion

        #region CÓDIGOS DO SISTEMA

        public static readonly CodigoSistema COD_26600 = new CodigoSistema("26600","", "Conexão funcionando corretamente");
        public static readonly CodigoSistema COD_26601 = new CodigoSistema("26601","", "Houve um erro de conexão");
        public static readonly CodigoSistema COD_74700 = new CodigoSistema("74700","", "O Terminal não está aberto");
        public static readonly CodigoSistema COD_38301 = new CodigoSistema("38301","", "Cópia de carro forçada");

        #endregion

        #region PROJETO_CONIFG

        public static readonly string GATEWAY_SERVER_VERSION = "1.0.0";
        public static readonly string GATEWAY_TERMINAL_VERSION = "1.0.0";
        public static readonly string GATEWAY_SERVER_INTERFACE_VERSION = "1.0.0";
        public static readonly string DESENVOLVEDOR_PROJETO = "Rio Service Company";
        
        public static readonly string ATUALIZACAO_SERVIDOR_FTP = "rservice.dnsalias.com.br";
        public static readonly string FTP_DIRETORIO_ROOT = "/busvision_gateway/";
        public static readonly string FTP_DIRETORIO_SERVER = FTP_DIRETORIO_ROOT + "server/";
        public static readonly string FTP_DREITORIO_TERMINAL = FTP_DIRETORIO_ROOT + "terminal/";
        public static readonly string FTP_DIRETORIO_INTERFACE = FTP_DIRETORIO_ROOT + "interface/";

        public static readonly string FTP_USUARIO = "rservice";
        public static readonly string FTP_SENHA = "nada00";



        #endregion

        #region VALORES_PADRAO

        public static readonly string DEFAULT_MANDANTE_COPIA = "Busvision Gateway Server";
        
        #endregion

        public static readonly string EMAIL_MALA_DIRETA_ENDERECO = "rservice@rioservice.com";
        public static readonly string EMAIL_MALA_DIRETA_SENHA = "rscompany";

        public static readonly string IMAGEM_CARRO_BRANCO = "/carros_img/carro_branco.png";
        public static readonly string IMAGEM_CARRO_VERDE = "/carros_img/carro_verde.png";
        public static readonly string IMAGEM_CARRO_VERDE_ESCURO = "/carros_img/carro_verde_escuro.png";
        public static readonly string IMAGEM_CARRO_AMARELO = "/carros_img/carro_amarelo.png";
        public static readonly string IMAGEM_CARRO_LARANJA = "/carros_img/carro_laranja.png";
        public static readonly string IMAGEM_CARRO_VERMELHO = "/carros_img/carro_vermelho.png";
        public static readonly string IMAGEM_CARRO_AZUL = "/carros_img/carro_azul.png";
        public static readonly string IMAGEM_CAMERA_TRANSPARENTE = "/carros_img/cam_transparente.png";
        public static readonly string IMAGEM_CAMERA_VERMELHA = "/carros_img/cam_vermelha.gif";
        public static readonly string IMAGEM_CAMERA_AMARELA = "/carros_img/cam_amarela.gif";

        public static readonly string AREA_TRABALHO = @"C:\rioservice\ftp\";
        public static readonly int QT_COPIAS_SIMULTANEAS = 10;
        public static readonly int QT_CAMERAS_PADRAO = 2;
        public static readonly string AREA_TRABALHO_MANUTENCAO = AREA_TRABALHO + @"Manutencao\";
        public static readonly string AREA_TRABALHO_TRANSFERENCIA = AREA_TRABALHO + @"Transferencia\";
        public static readonly string CAMINHO_GATEWAY_SERVER = @"C:\Rio Service\Gateway\Server\ProjetoFTP.Central.Manutencao.exe";
        public static readonly string CAMINHO_GATEWAY_CHECKER = @"C:\Rio Service\Gateway\Server\ProjetoFTP.Central.Verificador.exe";
        public static readonly string CAMINHO_GATEWAY_UPDATER = @"C:\Rio Service\Gateway\Server\ProjetoFTP.Central.Atualizador.exe";
        public static readonly string CAMINHO_TERMINAL = @"C:\Rio Service\Terminal\ProjetoFTP.Terminal.exe";
        public static readonly string FORMATO_DIRETORIO_CARRO_INTERFACE = AREA_TRABALHO + @"\{1}\{2}.info";
        public static readonly string CAMINHO_EMAILS = AREA_TRABALHO + "emails.txt";
        public static readonly string TERMINAL_LOCALHOST = DIRETORIO_TERMINAIS + "localhost.trm";

        public static readonly string XML_LISTA_ESTADOS_MANUTENCAO = AREA_TRABALHO_MANUTENCAO + "xml_lista_estados.xml";
        public static readonly string XML_LISTA_ESTADOS_TRANSFERENCIA = AREA_TRABALHO_TRANSFERENCIA + "xml_lista_estados.xml";
        public static readonly string XML_LISTA_CARROS_MANUTENCAO = AREA_TRABALHO_MANUTENCAO + "xml_lista_carros.xml";
        public static readonly string XML_AVISO_INTERFACE_MANUTENCAO = AREA_TRABALHO + "xml_aviso_interface.xml";
        public static readonly string XML_AVISO_INTERFACE_TRANSFERENCIA = AREA_TRABALHO + "xml_lista_estados.xml";
        public static readonly string DAT_ARQUIVO_CONFIGURACAO = AREA_TRABALHO + "config.dat";
        public static readonly string ARQUIVO_VERSAO = AREA_TRABALHO + "version.cv";
        public static readonly string JSON_LISTA_TERMINAIS = AREA_TRABALHO + "terminais.json";
        public static readonly string JSON_SERVER_STATUS = AREA_TRABALHO + "srvstats.json";
        public static readonly string INFO_GATEWAY_SERVER_UPDATE = AREA_TRABALHO + "bgsupdate.info";

        public static readonly string DIRETORIO_LOGS = AREA_TRABALHO + @"logs\";
        public static readonly string DIRETORIO_LOGS_USUARIOS = AREA_TRABALHO + @"logs_usu\";
        public static readonly string DIRETORIO_TERMINAIS = AREA_TRABALHO + @"\servidor\terminais\";
        public static readonly string DIRETORIO_HISTORICO_PAINEL = AREA_TRABALHO + @"painel_historico\";
        public static readonly string DIRETORIO_IMAGENS = @"c:\imagens\";
        public static readonly string ARQUIVO_CONFIGURACAO = AREA_TRABALHO + "sys.config";
        public static readonly string DIRETORIO_TRANSFERENCIA_PARCIAL = DIRETORIO_TERMINAIS + @"transferencias\";
        public static readonly int INTERVALO_VERIFICACAO = 720;

        public static readonly string IP_CENTRAL = File.ReadAllLines(CamadaConfiguracao.ARQUIVO_CONFIGURACAO)[5];
        
        public static ServerResponse GetServerResponse(string codigo)
        {
            ServerResponse result = new ServerResponse();

            switch (codigo)
            {
                case "200":
                    result = new ServerResponse()
                    {
                        Codigo = codigo,
                        Status = "OK",
                        Resposta = "Transferência concluída com sucesso"
                    };
                    break;
                case "300":
                    result = new ServerResponse()
                    {
                        Codigo = codigo,
                        Status = "Rede",
                        Resposta = "Erro de conexão de rede",
                        Procedimentos = new string[] {
                            "Verificar se o veículo está no alcance da antena.",
                            "Verificar ping manualmente.",
                            "Verificar se os cabos de rede estão conectados corretamente.",
                            "Verificar se há algum problema na rede.",
                            "Verificar se o DVR está travado.",
                            "Verificar se o DVR está desligado.",
                            "Caso tudo esteja funcionando, force a cópia."
                        }
                    };
                    break;

                case "400":
                    result = new ServerResponse()
                    {
                        Codigo = codigo,
                        Status = "FTP",
                        Resposta = "Erro de conexão FTP",
                        Procedimentos = new string[] {
                            "Verificar se o veículo ainda está no alcance da antena.",
                            "Verificar se os cabos de rede estão conectados corretamente.",
                            "Acessar a interface do DVR e verificar se o FTP esta habilitado.",
                            "Testar conexão FTP manualmente",
                            "Certificar-se de que o usuário e senha da interface corresponde com os cadastrados no sistema.",
                            "Verificar se o DVR estava travado.",
                            "Verificar se o DVR estava desligado."
                        }
                    };
                    break;

                case "500":
                    result = new ServerResponse()
                    {
                        Codigo = codigo,
                        Status = "Equipamento",
                        Resposta = "Equipamento não contém arquivos",
                        Procedimentos = new string[] {
                            "Verificar se o DVR estava gravando.",
                            "Verificar se o DVR está travado.",
                            "Verificar se o DVR está ligado.",
                            "Verificar se o FTP funcionando normalmente.",
                            "Certifique-se de que realmente não há arquivos no equipamento.",
                            "Caso exista arquivos, force a cópia."
                        }
                    };
                    break;

                case "1000":
                    result = new ServerResponse()
                    {
                        Codigo = codigo,
                        Status = "Desconhecido",
                        Resposta = "Erro desconhecido pelo sistema.",
                        Procedimentos = new string[] {
                            "Verificar conexão de rede no veículo e no terminal.",
                            "Acessar interface do DVR e verficar se a conexão FTP está habilitada.",
                            "Certificar-se de que o usuário e senha da interface corresponde com os cadastrados no sistema.",
                            "Realizar teste manual de conexão FTP.",
                            "Verificar se os cabos de rede estão conectados corretamente.",
                            "Reportar erro para o desenvolvedor.",
                            "Caso tudo esteja funcionando, force a cópia."
                        }
                    };
                    break;
            }
            return result;
        }

        public static ParametrosConfiguracao GetConfiguracoes()
        {
            string fileText = File.ReadAllText(@"c:\rioservice\ftp\config.json");
            var auxJson = new
            {
                ora_host = "",
                ora_port = 0,
                ora_user = "",
                ora_pass = "",
                ora_servicename = "",
                server_host = "",
                terminal_host = ""
            };
            var json = JsonConvert.DeserializeAnonymousType(fileText,auxJson);
            ParametrosConfiguracao configuracao = new ParametrosConfiguracao
            {
                OracleHost = json.ora_host,
                OraclePort = json.ora_port,
                OraclePass = json.ora_pass,
                OracleUser = json.ora_user,
                ServiceName = json.ora_servicename,
                ServerHost = json.server_host,
                TerminalHost = json.terminal_host
            };
            return configuracao;
        }
    }

    public class CodigoSistema
    {
        public string Id { get { return this._id; } }
        public string Tag { get { return this._tag; } }
        public string Mensagem { get { return this._mensagem; } }
        private string _id;
        private string _mensagem;
        private string _tag;

        public CodigoSistema(string id, string tag, string mensagem)
        {
            this._id = id;
            this._mensagem = mensagem;
            this._tag = tag;
        }


    }
    
    public class EventLog
    {
        public DateTime Data { get { return this._data; } }
        public string Codigo { get { return this._codigo; } }
        public string Tag { get { return this._tag; } }
        public string Mensagem { get { return this._mensagem; } }
       
        private DateTime _data;
        public string _codigo;
        public string _tag;
        public string _mensagem;

        public EventLog(DateTime data, string codigo, string tag, string mensagem)
        {
            this._data = data;
            this._codigo = codigo;
            this._tag = tag;
            this._mensagem = mensagem;
        }

        public static EventLog Parse(string log)
        {
            try
            {
                string[] param = log.Split(';');
                DateTime data = new DateTime(Convert.ToInt32(param[0].Substring(0, 4)),
                                             Convert.ToInt32(param[0].Substring(4, 2)),
                                             Convert.ToInt32(param[0].Substring(6, 2)),
                                             Convert.ToInt32(param[0].Substring(8, 2)),
                                             Convert.ToInt32(param[0].Substring(10, 2)),
                                             Convert.ToInt32(param[0].Substring(12, 2)));
                string codigo = param[1];
                string tag = param[2];
                string mensagem = param[3];
                EventLog result = new EventLog(data, codigo, tag, mensagem);
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }

    public class ServerResponse
    {
        public string Codigo { get; set; }
        public string Status { get; set; }
        public string Resposta { get; set; }
        public string[] Procedimentos { get; set; }
    }

    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Permissao { get; set; }
    }

    public class ParametrosConfiguracao
    {
        public string OracleHost { get; set; }
        public int OraclePort { get; set; }
        public string OracleUser { get; set; }
        public string OraclePass { get; set; }
        public string ServiceName { get; set; }
        public string ServerHost { get; set; }
        public string TerminalHost { get; set; }
    }

}
