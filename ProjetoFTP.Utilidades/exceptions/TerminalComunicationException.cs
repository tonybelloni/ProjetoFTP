using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjetoFTP.Utilidades.exceptions
{
    public class TerminalComunicationException : Exception
    {
        public TerminalComunicationException()
        {
        }

        public TerminalComunicationException(string message)
            : base(message)
        {
        }
    }
}
