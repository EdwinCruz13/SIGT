using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Ticket
    {
        public string Mov { get; set; }
        public string CodTicket { get; set; }
        public Cliente Cliente { get; set; }
        public Motivo Motivo { get; set; }
        public String FechaSolicitud { get; set; }
        public string Observaciones { get; set; }
        public int Estado { get; set; }

        //si existe algun usuario que atendió el ticket
        public CapaEntidad.Usuario Usuario { get; set; }



        // ---------------------------------------------
        public string EstadoDesc { get; set; }
        public string PrioridadDesc { get; set; }

        //------------------------------------------
        public string TiempoInicia { get; set; }
        public string TiempoFinaliza { get; set; }
    }
}
