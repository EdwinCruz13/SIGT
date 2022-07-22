using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class EstacionMotivos
    {
        public CapaEntidad.Estacion Estacion { get; set; }
        public List<CapaEntidad.Motivo> Motivos { get; set; }
    }
}
