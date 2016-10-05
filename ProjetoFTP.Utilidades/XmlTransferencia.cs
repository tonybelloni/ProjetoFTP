using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjetoFTP.Utilidades
{   
    [Serializable]
    public class XmlTransferencia
    {
        public int Numero { get; set; }
        public int ArquivosCopiados { get; set; }
        public int ArquivosTotal { get; set; }
        public double VolumeCopiado { get; set; }
        public double VolumeTotal { get; set; }
        public double VelocidadeMedia { get; set; }
        public DateTime DataInicial { get; set; }
    }
}
