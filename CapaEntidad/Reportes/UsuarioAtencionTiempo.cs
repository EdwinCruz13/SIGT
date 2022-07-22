using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class UsuarioAtencionTiempo
    {
        public String Usuario { get; set; }
        public String IdArea { get; set; }

        public PivotePrestamo_Tiempo TiempoPrestamos { get; set; }
        public PivoteRecuperaciones_Tiempo TiempoRecuperaciones { get; set; }
    }

}
