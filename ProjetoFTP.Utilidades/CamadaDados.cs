using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using System.IO;
using System.Xml.Serialization;

namespace ProjetoFTP.Utilidades
{
    public class CamadaDados
    {

        private string _connectionstring;
        private string _ip;
        private string _user;
        private string _pass;
        private string _servicename;
        private string _port;
        private OracleConnection _connection;
        private OracleCommand _command;
        private OracleDataAdapter _dataadapter;
        private DataSet _dataset;
        private static Log execlog;

        public CamadaDados()
        {

            this._ip = File.ReadAllLines(CamadaConfiguracao.ARQUIVO_CONFIGURACAO)[0];

            this._port = File.ReadAllLines(CamadaConfiguracao.ARQUIVO_CONFIGURACAO)[1];
            this._user = File.ReadAllLines(CamadaConfiguracao.ARQUIVO_CONFIGURACAO)[2];
            this._pass = File.ReadAllLines(CamadaConfiguracao.ARQUIVO_CONFIGURACAO)[3];
            this._servicename = File.ReadAllLines(CamadaConfiguracao.ARQUIVO_CONFIGURACAO)[4];

            //_connectionstring = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + _ip + ")(PORT=" + _port + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + _servicename + ")));User Id=" + _user + ";Password=" + _pass;
            _connectionstring = String.Format("user id=rs;password=rs0n1bus;data source={0}:{1}/{2}", _ip, _port, _servicename);

            execlog = new Log();
        }

        public DataSet RealizaConsultaSql(string query)
        {
            try
            {
                // Antonio Para não aparecer a string de conexão : Console.WriteLine(_connectionstring);
            
                _connection = new OracleConnection(_connectionstring);
                _connection.Open();

                _command = new OracleCommand()
                {
                    Connection = _connection,
                    CommandType = CommandType.Text,
                    CommandText = query
                };

                _dataadapter = new OracleDataAdapter(_command);
                _dataset = new DataSet();
                _dataadapter.Fill(_dataset);

                //execlog.GravalogExec("sql.log", "CamadaDados - Query : " + query);
                return _dataset;
            }
            catch (OracleException ex)
            {
                //execlog.GravalogExec("sql.log", "CamadaDados - OracleException : " + ex.Message + ":" + _connectionstring + Environment.NewLine + ex.StackTrace);
                throw new Exception("CamadaDados - RealizaConsultaSQL : " + ex.Message);
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("sql.log", "CamadaDados - RealizaConsultaSQL : " + ex.Message + ":" + _connectionstring + Environment.NewLine + ex.StackTrace);    
                throw new Exception("CamadaDados - RealizaConsultaSQL : " + ex.Message);
            }
            finally
            {
                ResetaObjetos();
            }
        }

        public void RealizaConsultaSqlVoid(string query)
        {
            try
            {
                _connection = new OracleConnection(_connectionstring);
                _connection.Open();
                _command = new OracleCommand()
                {
                    Connection = _connection,
                    CommandType = CommandType.Text,
                    CommandText = query
                };
                _dataadapter = new OracleDataAdapter(_command);
                _dataset = new DataSet();
                _dataadapter.Fill(_dataset);
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("sql.log", "CamadaDados - RealizaConsultaSQLVoid : " + ex.Message);
                throw new Exception("CamadaDados - RealizaConsultaSQLVoid : " + ex.Message);
                throw ex;
            }
            finally
            {
                ResetaObjetos();
            }
        }

        public void ConcatenaItemTrm(string path, string value)
        {
            try
            {
                string newLine = Criptografia.Encrypt(value, true);
                EscreveArquivoTxt(path, newLine);
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("sql.log", "CamadaDados - ConcatenaItemTrm : " + ex.Message);
                throw new Exception("CamadaDados - ConcatenaItemTrm : " + ex.Message);
            }
        }

        public List<string> LerArquivoTrm(string path)
        {
            try
            {
                string[] result = null;

                if (File.Exists(path))
                {
                    result = File.ReadAllLines(path);
                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = Criptografia.Decrypt(result[i], true);
                    }
                }
                else
                {
                    //execlog.GravalogExec("servidor.log", string.Format("CamadaDados - LerArquivoTrm - Arquivo {0} não pode ser lido", path));
                }

                return result.ToList();
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("sql.log", "CamadaDados - LerArquivoTrm :" + ex.Message);
                throw new Exception("CamadaDados - LerArquivoTrm : " + ex.Message);
                //return LerArquivoTrm(path);
            }
        }

        public void RemoveLinhaTrm(string path, string value)
        {
            try
            {
                string target = Criptografia.Encrypt(value, true);
                RemoveLinhaTxt(path, value);
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("sql.log", "CamadaDados - RemoveLinhaTrm : " + ex.Message);
                throw new Exception("CamadaDados - RemoveLinhaTrm : " + ex.Message);
            }
        }
        
        public void RemoveLinhaTxt(string path, string value)
        {
            try
            {
                string[] linhas = GetLinhasTxt(path);
                List<string> linhasNovas = new List<string>();
                string encryptValue = Criptografia.Encrypt(value, true);
                for (int i = 0; i < linhas.Length; i++)
                {
                    if (!linhas[i].Contains(encryptValue))
                        linhasNovas.Add(linhas[i]);
                }
                File.WriteAllLines(path, linhasNovas.ToArray());
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("sql.log", "CamadaDados - RemoveLinhaTxt : " + ex.Message);
                throw new Exception("CamadaDados - RemoveLinhaTxt : " + ex.Message);
                //RemoveLinhaTxt(path, value);
            }
        }

        public string[] GetLinhasTxt(string path)
        {
            try
            {
                return File.ReadAllLines(path);
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("sql.log", "CamadaDados - GetLinhasTxt : " + ex.Message);
                throw new Exception("CamadaDados - GetLinhasTxt : " + ex.Message);
                //return GetLinhasTxt(path);
            }
        }

        public void EscreveArquivoTxt(string path, string value)
        {
            try
            {
               // execlog.GravalogExec("sql.log", "CamadaDados - EscreveArquivoYxt - Carro : " + value + " - Path : " + path);

                StreamWriter sw = File.AppendText(path);
                sw.WriteLine(value);
                sw.Flush();
                sw.Dispose();
                sw.Close();
                sw = null;
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("sql.log", "CamadaDados - EscreveArquivoTxt : " + ex.Message);
                //throw new Exception("CamadaDados - EscreveArquivoTxt : " + ex.Message);
                EscreveArquivoTxt(path, value);
            }
        }

        private void ResetaObjetos()
        {
            if (_command != null)
               _command.Dispose();

            if (_dataadapter != null)
               _dataadapter.Dispose();

            if (_dataset != null)
               _dataset.Dispose();

            if (_connection != null)
               _connection.Close();

            if (_connection != null)
               _connection.Dispose();
            
            _command = null;
            _dataadapter = null;
            _dataset = null;
        }

        public static string LerArquivo(string path)
        {
            string contents = string.Empty;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        contents = reader.ReadToEnd();
                        reader.Close();
                    }
                    fs.Close();
                }
                return contents;
            }
            catch(Exception ex)
            {
                contents = ex.Message;
                return contents;
            }
        }

        public static void EscreveArquivo(string path, string contents)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(contents);
                        writer.Flush();
                        writer.Close();
                    }
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("sql.log", "CamadaDados - EscreveArquivo : " + ex.Message);
                throw new Exception("CamadaDados - EscreveArquivo : " + ex.Message);
                Console.WriteLine(DateTime.Now.ToString("dd/MM - HH:mm") + " - Erro ao escrever no arquivo: " + path);
            }
        }
    }
}
