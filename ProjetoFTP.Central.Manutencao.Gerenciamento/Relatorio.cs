using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetoFTP.Web
{

    public struct RelatorioCopiaItem
    {
        public int Id { get; set; }
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
                return (VolumeArquivosCopiados == 0) ? 0 : ((Intervalo * 60) == 0) ? VolumeArquivosCopiados : VolumeArquivosCopiados / Intervalo;
            }
        }
        public string TipoCopia { get; set; }
        public string Codigo { get; set; }
        public string Usuario { get; set; }
        public string Estacao { get; set; }
        public string DataInicialString { get { return DataInicial.ToString("dd/MM/yyyy HH:mm:ss"); } }
        public string DataFinalString { get { return DataFinal.ToString("dd/MM/yyyy HH:mm:ss"); } }
        public string PeriodoInicialString { get { return PeriodoInicial.ToString("dd/MM/yyyy HH:mm:ss"); } }
        public string PeriodoFinalString { get { return PeriodoFinal.ToString("dd/MM/yyyy HH:mm:ss"); } }
    }

    public struct RelatorioVerificacaoItem
    {
        public int NumeroCarro { get; set; }
        public string CodigoEquipamento { get; set; }
        public DateTime DataVerificacao { get; set; }
        public int QtCam1 { get; set; }
        public int QtCam2 { get; set; }
        public int QtCam3 { get; set; }
        public int QtCam4 { get; set; }
        public double Cam1 { get; set; }
        public double Cam2 { get; set; }
        public double Cam3 { get; set; }
        public double Cam4 { get; set; }
    }
}