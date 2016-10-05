using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ProjetoFTP.Utilidades
{
    public class ResultadoTransferencia
    {
        public int Numero { get; set; }
        public int ArquivosCopiados { get; set; }
        public double VolumeCopiado { get; set; }
        public double VolumeTotal { get; set; }
        public double Percent { get { return (VolumeCopiado / VolumeTotal) * 100; } }
        public double VelocidadeMedia { get; set; }
        public int ArquivosTotal { get; set; }
        public string DataInicial { get; set; }
    }

    public class Transferencia
    {
        public static void Atualiza(ResultadoTransferencia transferencia)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://" + CamadaConfiguracao.IP_CENTRAL + "/queries/GetTransferencia.ashx?a=att&n=" + transferencia.Numero);
                request.ContentType = "text/json";
                request.Method = "POST";
                request.KeepAlive = true;
                StreamWriter requestStream = new StreamWriter(request.GetRequestStream());
                string postString = JsonConvert.SerializeObject(transferencia);
                requestStream.WriteLine(postString);
                requestStream.Flush();
                requestStream.Close();
                WebResponse response = request.GetResponse();
                response.Close();
                request = null;
            }
            catch(Exception ex)
            {
            }
        }

        public static void Remove(ResultadoTransferencia transferencia)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://" + CamadaConfiguracao.IP_CENTRAL + "/queries/GetTransferencia.ashx?a=del&n=" + transferencia.Numero);
            request.ContentType = "text/json";
            WebResponse response = request.GetResponse();
            response.Close();
            request = null;
        }

    }

}
