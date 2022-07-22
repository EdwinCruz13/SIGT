using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Turno
{
    public class Llamado
    {
        public string CodTicket { get; set; }
        public string Ticket { get; set; }
        public string IdZona { get; set; }
        public string Estacion { get; set; }
        public string IdEstado { get; set; }
    }
}
