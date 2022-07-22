using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class PivotTicket_AtencionUsuario
    {
        public String Usuario { get; set; }
        public int Anulados { get; set; }
        public int Pendientes { get; set; }
        public int Asignados { get; set; }
        public int Procesando { get; set; }
        public int Atendidos { get; set; }
        public int Total { get; set; }
    }
}
