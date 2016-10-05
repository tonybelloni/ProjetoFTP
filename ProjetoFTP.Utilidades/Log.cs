using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace ProjetoFTP.Utilidades
{
    public enum TiposEvento
    {
        Erro = 0, Usuário = 1, FalhaStream = 2, DispositivoDesconectado = 3,
        ConfiguraçãoErrada = 4, ConfiguraçãoAlterada = 5, CâmeraEditada = 6,
        CâmeraDeletada = 7, CâmeraInserida = 8, UsuárioDeletado = 9, UsuárioInserido = 10,
        UsuárioDesconectado = 11, UsuárioConectado = 12, InicializaçãoSistema = 13, FinalizaçãoSistema = 14, DetecçãoMovimento = 15, Alerta = 16, UsuarioAlterado = 17, DuracaoErro = 18,
        BackupRealizado = 19, FaltaBackup = 20, BackupInterrompido = 21, DuracaoExecucao = 22, AtualizacaoSistema = 23
    }

    public class Log
    {
        private string _caminho;
        private StreamWriter _stream;

        public Log()
        {
        }

        public Log(string caminho)
        {
            this._caminho = caminho;
            string pasta = _caminho.Replace(_caminho.Split('\\').Last(), string.Empty);
            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);
        }

       public void GravalogExec(String arquivo, String mensagem)
        {
            var fs = new FileStream(String.Format("c:\\temp\\{0}", arquivo), FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

            using (StreamWriter writer = new StreamWriter(fs, Encoding.Default))
            {
                writer.WriteLine(String.Format("{0} - {1}", DateTime.Now.ToString(), mensagem));
                writer.Close();
            }
        }

        public void AdicionaEvento(string evento, bool encrypt)
        {
            try
            {
                string target = evento.ToString();
                StreamWriter sw = File.AppendText(_caminho);
                sw.WriteLine(target);
                sw.Flush();
                sw.Close();
                sw.Dispose();
                sw = null;
            }
            catch
            {
                AdicionaEvento(evento, encrypt);
            }
        }

        public void AddLog(string msg)
        {
            this._stream = new StreamWriter(this._caminho, true);
            this._stream.Write(string.Format("{0},{1}\r\n", msg, DateTime.Now));
            this._stream.Flush();
            this._stream.Close();
            this._stream.Dispose();
        }

        public static void AdicionaEvento(string caminho, string evento, bool encrypt)
        {
            try
            {
                string target = string.Empty;
                if (encrypt)
                    target = Criptografia.Encrypt(evento.ToString(), true);
                else
                    target = evento.ToString();
                StreamWriter sw = File.AppendText(caminho);
                sw.WriteLine(target);
                sw.Flush();
                sw.Close();
                sw.Dispose();
                sw = null;
            }
            catch
            {
                AdicionaEvento(caminho, evento, encrypt);
            }
        }
        
    }


}
