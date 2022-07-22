using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Zona
    {
        public string IdZona { get; set; }
        public string Siglas { get; set; }
        public string Descripcion { get; set; }
        public string Responsable { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int Estado { get; set; }
    }
}
