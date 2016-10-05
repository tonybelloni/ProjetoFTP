using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace ProjetoFTP.Utilidades
{
    public class FTP
    {
        private FtpWebRequest _request;
        private List<string> _filesListed;
        private Log execlog;

        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public FTP(string server, string user, string pass)
        {
            this.Server = server;
            this.User = user;
            this.Password = pass;
            this._filesListed = new List<string>();
            execlog = new Log();

        }

        public bool IsConnecting()
        {
            try
            {
                InitializeRequest("");
                this._request.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;
                FtpWebResponse response = (FtpWebResponse)this._request.GetResponse();
                response.Close();
                this._request = null;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ListFiles(string ftpDir, List<string> arqs)
        {
            string[] details = GetListDetails(ftpDir);

            for (int i = 0; i < details.Length; i++)
            {
                string line = details[i];
                if (!string.IsNullOrEmpty(line))
                {
                    try
                    {
                        string[] tokens = line.Split(' ');
                        if (IsDetailFolder(line))
                        {
                            //list again
                            string targetDir = ftpDir + "/" + tokens[tokens.Length - 1];
                            ListFiles(targetDir, arqs);
                        }
                        else
                        {
                            //is file
                            ClearTokens(ref tokens);
                            string file = tokens[tokens.Length - 1] + ";" + tokens[4];
                            arqs.Add((string.IsNullOrEmpty(ftpDir) ? ftpDir : ftpDir + "/") + file);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void Download(string ftpFileName, string remoteFileName)
        {

            try
            {
                using (FileStream outputStream = new FileStream(remoteFileName, FileMode.Create))
                {
                    this.InitializeRequest(ftpFileName);
                    this._request.UsePassive = true;
                    this._request.KeepAlive = true;
                    this._request.UseBinary = true;
                    this._request.Method = WebRequestMethods.Ftp.DownloadFile;
                    using (WebResponse response = this._request.GetResponse())
                    {
                        using (Stream ftpStream = response.GetResponseStream())
                        {
                            int bufferSize = 262144;
                            byte[] buffer = new byte[bufferSize];
                            int readCount = ftpStream.Read(buffer, 0, bufferSize);
                            while (readCount > 0)
                            {
                                outputStream.Write(buffer, 0, readCount);
                                readCount = ftpStream.Read(buffer, 0, bufferSize);
                            }
                            ftpStream.Close();
                            outputStream.Close();
                        }
                        response.Close();
                    }
                }
                this._request = null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public string GetBannerMessage()
        {
            try
            {
                //execlog.GravalogExec("servidor.log", "Carro - GetBannerMessage - InitializeRequest");

                InitializeRequest("");
                this._request.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;
                this._request.UseBinary = false;
                this._request.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)this._request.GetResponse();
                string banner = response.BannerMessage;
                response.Close();
                this._request = null;
                return banner;
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("servidor.log", "FTP - GetBannerMessage : " + ex.Message);
                return null;
            }
        }

        private void InitializeRequest(string path)
        {
            try
            {
                // DEBUG : execlog.GravalogExec("ftp.log", "SERVER = ftp://" + this.Server + "/" + path);

                this._request = (FtpWebRequest)WebRequest.Create("ftp://" + this.Server + "/" + path);
                this._request.Credentials = new NetworkCredential(this.User, this.Password);
                this._request.ConnectionGroupName = "busvisionftp";
                this._request.KeepAlive = true;
            }
            catch (Exception ex)
            {
                //execlog.GravalogExec("servidor.log", "FTP - InitializeRequest : " + ex.Message);
            }
        }

        private bool IsDetailFolder(string detail)
        {
            if (detail[0] == 'd')
            {
                return true;
            }
            return false;
        }

        private string[] GetListDetails(string folder)
        {

            folder = (folder == string.Empty) ? "/" : folder + "/";
            // DEBUG : execlog.GravalogExec("ftp.log", "FOLDER = : " + folder);

            InitializeRequest(folder);
            
            this._request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            this._request.UseBinary = false;
            FtpWebResponse response = (FtpWebResponse)this._request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            int bufferSize = 1024;
            char[] buffer = new char[bufferSize];
            int aux = reader.Read(buffer, 0, bufferSize);
            StringBuilder sb = new StringBuilder(new string(buffer));
            while (aux > 0)
            {
                buffer = new char[bufferSize];
                aux = reader.Read(buffer,0,bufferSize);
                sb.Append(buffer);
                //DEBUG : execlog.GravalogExec("ftp.log", "SB = " + sb.ToString());
            }
            sb = sb.Replace("\0", string.Empty);
            string[] lines = sb.ToString().Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                // DEBUG : execlog.GravalogExec("ftp.log", lines[i]);
                lines[i] = lines[i].TrimEnd('\r');
            }
            reader.Close();
            reader.Dispose();
            response.Close();
            return lines;
        }

        private void ClearTokens(ref string[] tokens)
        {
            int len = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] != "")
                    len++;
            }
            int aux = 0;
            string[] clearedTokens = new string[len];
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] != "")
                    clearedTokens[aux++] = tokens[i];
            }
            tokens = clearedTokens;
        }
    }

    public struct FTPResponseInfo
    {
    }
}
