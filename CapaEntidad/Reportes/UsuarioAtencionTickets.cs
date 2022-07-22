using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class UsuarioAtencionTickets
    {
        public String Usuario { get; set; }
        public String IdArea { get; set; }
        public PivotePrestamo_Tickets AtencionPrestamos { get; set; }
        public PivoteRecuperaciones_Tickets AtencionRecuperaciones { get; set; }

    }
}
