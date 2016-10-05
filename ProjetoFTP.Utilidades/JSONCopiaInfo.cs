using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjetoFTP.Utilidades
{
    public class JSONCopiaInfo
    {
        public int NumeroCarro { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public double Intervalo { get { return (DataFinal - DataInicial).TotalMinutes; } }
        public int QuantidadeArquivosTotal { get; set; }
        public int QuantidadeArquivosValidos { get; set; }
        public int QuantidadeArquivosCopiados { get; set; }
        public Int64 VolumeArquivosTotal { get; set; }
        public Int64 VolumeArquivosCopiados { get; set; }
        public string PenDrive { get; set; }
        public DateTime PeriodoInicial { get; set; }
        public DateTime PeriodoFinal { get; set; }
        public double VelocidadeMedia
        {
            get
            {
                return (VolumeArquivosCopiados == 0) ? 0 : (Intervalo == 0) ? VolumeArquivosCopiados : VolumeArquivosCopiados / Intervalo;
            }
        }
        public string TipoCopia { get; set; }
        public string Codigo { get; set; }
        public string Usuario { get; set; }
    }
}
